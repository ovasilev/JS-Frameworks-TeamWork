using News.Data;
using News.Services.Models;
using News.Services.Atributes;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ValueProviders;

namespace News.Services.Controllers
{
    public class TagsController : BaseApiController
    {
        public TagsController()
            : base()
        {
        }

        public TagsController(IDbContextFactory<DbContext> contextFactory)
            : base(contextFactory)
        {
        }

        [HttpGet]
        public IQueryable<TagModel> GetAll([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    this.ValidateSessionKey(sessionKey);

                    var context = this.contextFactory.Create() as NewsContext;
                    var tags = context.Tags.OrderBy(t => t.Name);

                    var result = from tag in tags
                                 select new TagModel()
                                 {
                                     Name = tag.Name,
                                     Id = tag.Id,
                                     Count = tag.Articles.Count
                                 };

                    return result.OrderBy(t => t.Name);
                });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("articles")]
        public IQueryable<ArticleModel> GetArticlesByTagId(int tagId)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = this.contextFactory.Create() as NewsContext;

                    var tag = context.Tags.Where(t => t.Id == tagId).FirstOrDefault();

                    if (tag == null)
                    {
                        throw new ArgumentNullException("Invalid tag Id!");
                    }

                    var responce = from articleEntity in tag.Articles.AsQueryable()
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

        [HttpGet]
        [ActionName("articles")]
        public IQueryable<ArticleModel> GetPagedArticlesByTagId(int tagId, int page, int count)
        {
            var models = this.GetArticlesByTagId(tagId)
                .Skip(page * count)
                .Take(count);
            return models;
        }
    }
}
