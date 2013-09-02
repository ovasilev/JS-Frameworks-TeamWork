using News.Data;
using News.Models;
using News.Services.Models;
using News.Services.Atributes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;

namespace News.Services.Controllers
{
    public class ArticlesController : BaseApiController
    {
        public ArticlesController()
            : base()
        {
        }

        public ArticlesController(IDbContextFactory<DbContext> contextFactory)
            : base(contextFactory)
        {
        }

        [HttpGet]
        public IQueryable<ArticleModel> GetAll()
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                 () =>
                 {
                     var context = this.contextFactory.Create() as NewsContext;

                     var articleEntities = context.Articles
                         .Include(art => art.Author)
                         .Include(art => art.Image)
                         .Include(art => art.Comments)
                         .Include(art => art.Tags);

                     var model = from article in articleEntities
                                 select new ArticleModel()
                                 {
                                     Id = article.Id,
                                     Title = article.Title,
                                     ContentPreview = article.Content,
                                     DatePublished = article.DatePublished,
                                     Author = article.Author.DisplayName,
                                     ThumbUrl = article.Image.ThumbUrl,
                                     Category = new CategoryModel
                                     {
                                         Id = article.Category.Id,
                                         Name = article.Category.Name,
                                     },
                                     ReadCount = article.ReadCount,
                                 };

                     return model.OrderByDescending(art => art.DatePublished);

                 });

            return responseMsg;
        }

        [HttpGet]
        public ArticleFullModel GetById(int id)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                 () =>
                 {
                     var context = this.contextFactory.Create() as NewsContext;

                     var article = context.Articles
                         .Include(art => art.Author)
                         .Include(art => art.Image)
                         .Include(art => art.Comments)
                         .Include(art => art.Tags)
                         .FirstOrDefault(art => art.Id == id);

                     if (article == null)
                     {
                         throw new InvalidOperationException("Article doesn't exist"); 
                     }

                     article.ReadCount++;
                     context.SaveChanges();

                     var model = new ArticleFullModel()
                                 {
                                     Id = article.Id,
                                     Title = article.Title,
                                     Content = article.Content,
                                     DatePublished = article.DatePublished,
                                     Author = article.Author.DisplayName,
                                     ImageUrl = article.Image.ThumbUrl,
                                     Category = new CategoryModel
                                     {
                                         Id = article.Category.Id,
                                         Name = article.Category.Name,
                                     },
                                     ReadCount = article.ReadCount,
                                     Tags = (from tag in article.Tags
                                             select tag.Name),
                                     Comments = (from comment in article.Comments
                                                 select new CommentModel()
                                                 {
                                                     CommentedBy = comment.User.DisplayName,
                                                     Content = comment.Content,
                                                     DatePosted = comment.DatePosted
                                                 })
                                 };

                     return model;

                 });

            return responseMsg;
        }

        [HttpGet]
        public IQueryable<ArticleModel> GetByKeyword(string keyword)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var models = this.GetAll().
                      Where(art => art.Title.ToLower().Contains(keyword.ToLower())).
                      OrderByDescending(art => art.DatePublished);

                  return models;
              });

            return responseMsg;
        }

        public IQueryable<ArticleModel> GetByTags(string tags)
        {
            if (tags == null)
            {
                throw new ArgumentNullException("Tags cannot be null");
            }

            string[] tagNames = tags.ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var models = this.GetAll();
            foreach (var tagName in tagNames)
            {
                models = models.Where(art => art.Tags.Contains(tagName));
            }

            return models;
        }

        //api/posts?page=5&count=3
        [HttpGet]
        public IQueryable<ArticleModel> GetPaged(int page, int count)
        {
            var models = this.GetAll()
                .Skip(page * count)
                .Take(count);
            return models;
        }

        [HttpGet]
        public IQueryable<ArticleModel> GetPagedByKeyword(string keyword, int page, int count)
        {
            var models = this.GetByKeyword(keyword)
                .Skip(page * count)
                .Take(count);
            return models;
        }

        [HttpGet]
        public IQueryable<ArticleModel> GetPagedByTags(string tags, int page, int count)
        {
            var models = this.GetByTags(tags)
                .Skip(page * count)
                .Take(count);
            return models;
        }


        [HttpPost]
        public HttpResponseMessage PostArticle(ArticleFullModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                 () =>
                 {
                     var context = this.contextFactory.Create() as NewsContext;

                     using (context)
                     {
                         this.ValidateSessionKey(sessionKey);
                         this.ValidateArticleModel(model);

                         var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);
                         if (user == null)
                         {
                             throw new InvalidOperationException("User not logged in");
                         }

                         var category = context.Categories.FirstOrDefault(cat => cat.Id == model.Category.Id);
                         if (category == null)
                         {
                             throw new InvalidOperationException("Invalid category");
                         }

                         Article article = new Article()
                         {
                             Title = model.Title,
                             Content = model.Content,
                             DatePublished = DateTime.Now,
                             Author = user,
                             Category = category,
                             ReadCount = 0
                         };

                         foreach (var tagString in model.Tags)
                         {
                             var tagEntity = context.Tags.FirstOrDefault(tag => tag.Name == tagString.ToLower());

                             if (tagEntity == null)
                             {
                                 tagEntity = new Tag();
                                 tagEntity.Name = tagString.ToLower();
                                 context.Tags.Add(tagEntity);
                                 context.SaveChanges();
                             }
                             article.Tags.Add(tagEntity);
                         }

                         string[] titleTags = model.Title.Split(new char[] { ' ', '.', ',', '!', '?' },
                             StringSplitOptions.RemoveEmptyEntries);

                         foreach (var tagString in titleTags)
                         {
                             var tagEntity = context.Tags.FirstOrDefault(tag => tag.Name == tagString.ToLower());

                             if (tagEntity == null)
                             {
                                 tagEntity = new Tag();
                                 tagEntity.Name = tagString.ToLower();
                                 context.Tags.Add(tagEntity);
                                 context.SaveChanges();
                             }

                             if (!article.Tags.Contains(tagEntity))
                             {
                                 article.Tags.Add(tagEntity);
                             }
                         }

                         context.Articles.Add(article);
                         context.SaveChanges();

                         ArticleModel createdModel = new ArticleModel()
                         {
                             Id = article.Id,
                             Title = article.Title,
                             ContentPreview = article.Content.Length > 197
                                      ? article.Content.Substring(0, 197) + "..."
                                      : article.Content,
                             DatePublished = article.DatePublished,
                             Author = article.Author.DisplayName,
                             Category = new CategoryModel
                             {
                                 Id = category.Id,
                                 Name = category.Name,
                             },
                             ThumbUrl = article.Image != null ? article.Image.ThumbUrl : null,
                             ReadCount = article.ReadCount,
                         };

                         var response = this.Request.CreateResponse(HttpStatusCode.Created, createdModel);
                         return response;
                     }
                 });

            return responseMsg;
        }

        [HttpPut]
        public HttpResponseMessage PutArticle(ArticleFullModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                 () =>
                 {
                     var context = this.contextFactory.Create() as NewsContext;

                     using (context)
                     {
                         this.ValidateSessionKey(sessionKey);
                         this.ValidateArticleModel(model);

                         var article = context.Articles.FirstOrDefault(art => art.Id == model.Id);
                         if (article == null)
                         {
                             throw new InvalidOperationException("Article doesn't exist");
                         }

                         var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);
                         var category = context.Categories.FirstOrDefault(cat => cat.Id == model.Category.Id);

                         article.Title = model.Title;
                         article.Content = model.Content;
                         article.DatePublished = DateTime.Now;
                         article.Author = user;
                         article.Category = category;

                         foreach (var tagString in model.Tags)
                         {
                             var tagEntity = context.Tags.FirstOrDefault(tag => tag.Name == tagString.ToLower());

                             if (tagEntity == null)
                             {
                                 tagEntity = new Tag();
                                 tagEntity.Name = tagString.ToLower();
                                 context.Tags.Add(tagEntity);
                                 context.SaveChanges();
                             }

                             if (!article.Tags.Contains(tagEntity))
                             {
                                 article.Tags.Add(tagEntity);
                             }
                         }

                         string[] titleTags = model.Title.Split(new char[] { ' ', '.', ',', '!', '?' },
                             StringSplitOptions.RemoveEmptyEntries);

                         foreach (var tagString in titleTags)
                         {
                             var tagEntity = context.Tags.FirstOrDefault(tag => tag.Name == tagString.ToLower());

                             if (tagEntity == null)
                             {
                                 tagEntity = new Tag();
                                 tagEntity.Name = tagString.ToLower();
                                 context.Tags.Add(tagEntity);
                                 context.SaveChanges();
                             }

                             if (!article.Tags.Contains(tagEntity))
                             {
                                 article.Tags.Add(tagEntity);
                             }
                         }

                         //context.Articles.Add(article);
                         context.SaveChanges();

                         ArticleModel createdModel = new ArticleModel()
                         {
                             Id = article.Id,
                             Title = article.Title,
                             ContentPreview = article.Content.Length > 197
                                      ? article.Content.Substring(0, 197) + "..."
                                      : article.Content,
                             DatePublished = article.DatePublished,
                             Author = article.Author.DisplayName,
                             Category = new CategoryModel
                             {
                                 Id = category.Id,
                                 Name = category.Name,
                             },
                             ThumbUrl = article.Image.ThumbUrl,
                             ReadCount = article.ReadCount,
                         };

                         var response = this.Request.CreateResponse(HttpStatusCode.OK, createdModel);
                         return response;
                     }
                 });

            return responseMsg;
        }

        [HttpDelete]
        public HttpResponseMessage DeleteArticle(int articleId,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = this.contextFactory.Create() as NewsContext;

                    using (context)
                    {
                        this.ValidateSessionKey(sessionKey);

                        var article = context.Articles.FirstOrDefault(art => art.Id == articleId);
                        if (article == null)
                        {
                            throw new ArgumentException("Ivalid article Id");
                        }

                        foreach (var comment in article.Comments)
                        {
                            context.Comments.Remove(comment);
                        }

                        context.Articles.Remove(article);

                        if (article.Image != null)
                        {
                            context.Images.Remove(article.Image);
                        }

                        context.SaveChanges();
                    }

                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            return responseMsg;
        }

        [HttpPut]
        [ActionName("comment")]
        public HttpResponseMessage PutComment(int articleId, CommentModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = this.contextFactory.Create() as NewsContext;

                    using (context)
                    {
                        this.ValidateSessionKey(sessionKey);

                        var user = context.Users.Where(u => u.SessionKey == sessionKey).FirstOrDefault();
                        var article = context.Articles.Where(art => art.Id == articleId).FirstOrDefault();

                        if (article == null)
                        {
                            throw new ArgumentException("There is no such article!");
                        }

                        if (model.Content == null || model.Content == string.Empty)
                        {
                            throw new ArgumentNullException("Cannot create comment with empty content!");
                        }

                        var comment = new Comment()
                        {
                            Content = model.Content,
                            DatePosted = DateTime.Now,
                            User = user,
                            Article = article
                        };

                        context.Comments.Add(comment);
                        context.SaveChanges();

                        var responce = this.Request.CreateResponse(HttpStatusCode.Created);
                        return responce;
                    }
                });

            return responseMsg;
        }

        [HttpDelete]
        public HttpResponseMessage DeleteArticle(int articleId, int commentId,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = this.contextFactory.Create() as NewsContext;

                    using (context)
                    {
                        this.ValidateSessionKey(sessionKey);

                        var article = context.Articles.FirstOrDefault(art => art.Id == articleId);
                        if (article == null)
                        {
                            throw new ArgumentException("Ivalid article Id");
                        }

                        var comment = article.Comments.FirstOrDefault(com => com.Id == commentId);
                        if (comment == null)
                        {
                           throw new ArgumentException("Ivalid comment Id");
                        }

                        context.Comments.Remove(comment);
                        context.SaveChanges();
                    }

                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            return responseMsg;
        }

        private void ValidateArticleModel(ArticleFullModel model)
        {
            if (model == null)
            {
                throw new InvalidOperationException("Invalid article!");
            }

            if (model.Content == null)
            {
                throw new ArgumentNullException("Cannot create article with empty content!");
            }

            if (model.Title == null)
            {
                throw new ArgumentNullException("Cannot create article without title!");
            }

            if (model.Category == null)
            {
                throw new InvalidOperationException("Cannot create article without category");
            }
        }
    }
}
