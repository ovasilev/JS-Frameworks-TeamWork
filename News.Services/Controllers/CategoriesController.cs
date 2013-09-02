using News.Data;
using News.Models;
using News.Services.Models;
using News.Services.Atributes;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using System.Net.Http;
using System.Net;

namespace News.Services.Controllers
{
    public class CategoriesController : BaseApiController
    {
        public CategoriesController()
            : base()
        {
        }

        public CategoriesController(IDbContextFactory<DbContext> contextFactory)
            : base(contextFactory)
        {
        }

        [HttpGet]
        public IQueryable<CategoryModel> GetAll()
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = this.contextFactory.Create() as NewsContext;

                    var result = from category in context.Categories
                                 select new CategoryModel()
                                 {
                                     Id = category.Id,
                                     Name = category.Name,
                                 };

                    return result.OrderBy(t => t.Name);
                });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("posts")]
        public IQueryable<ArticleModel> GetById(int categoryId)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = this.contextFactory.Create() as NewsContext;

                    var category = context.Categories.Where(cat => cat.Id == categoryId).FirstOrDefault();

                    if (category == null)
                    {
                        throw new ArgumentNullException("Invalid category Id!");
                    }

                    var responce = from articleEntity in category.Articles.AsQueryable()
                                   select new ArticleModel()
                                   {
                                       Id = articleEntity.Id,
                                       Title = articleEntity.Title, 
                                       ContentPreview = articleEntity.Content.Length > 197
                                                ? articleEntity.Content.Substring(0, 197) + "..."
                                                : articleEntity.Content,
                                       DatePublished = articleEntity.DatePublished,
                                       Author = articleEntity.Author.DisplayName,
                                       Category = new CategoryModel
                                       {
                                           Id = articleEntity.Category.Id,
                                           Name = articleEntity.Category.Name,
                                       },
                                       ThumbUrl = articleEntity.Image.ThumbUrl,
                                       ReadCount = articleEntity.ReadCount,
                                   };

                    return responce.OrderByDescending(art => art.DatePublished);
                });

            return responseMsg;
        }

        [HttpPost]
        public HttpResponseMessage PostCategory(CategoryModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                 () =>
                 {
                     var context = this.contextFactory.Create() as NewsContext;

                     using (context)
                     {
                         this.ValidateSessionKey(sessionKey);
                         this.ValidateCategoryModel(model);

                         var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);
                         if (user == null)
                         {
                             throw new InvalidOperationException("User not logged in");
                         }

                         Category category = new Category()
                         {
                             Name = model.Name,
                         };

                         context.Categories.Add(category);
                         context.SaveChanges();

                         CategoryModel createdModel = new CategoryModel()
                         {
                             Id = category.Id,
                             Name = category.Name,
                         };

                         var response = this.Request.CreateResponse(HttpStatusCode.Created, createdModel);
                         return response;
                     }
                 });

            return responseMsg;
        }

        [HttpDelete]
        public HttpResponseMessage DeleteCategory(int categoryId,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = this.contextFactory.Create() as NewsContext;

                    using (context)
                    {

                        this.ValidateSessionKey(sessionKey);
                        var category = context.Categories.FirstOrDefault(cat => cat.Id == categoryId);
                        if (category == null)
                        {
                            throw new ArgumentException("Ivalid category Id");
                        }

                        foreach (var article in category.Articles)
                        {
                            foreach (var comment in article.Comments)
                            {
                                context.Comments.Remove(comment);
                            }

                            context.Articles.Remove(article);

                            if (article.Image != null)
                            {
                                context.Images.Remove(article.Image);
                            }
                        }

                        context.Categories.Remove(category);
                        context.SaveChanges();
                    }

                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            return responseMsg;
        }

        private void ValidateCategoryModel(CategoryModel model)
        {
            if (model == null)
            {
                throw new InvalidOperationException("Invalid category!");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentNullException("Cannot create article with empty content!");
            }
        }
    }
}
