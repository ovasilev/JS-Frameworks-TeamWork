using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace News.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "TagsApi",
                routeTemplate: "api/tags/{tagId}/articles",
                defaults: new
                {
                    controller = "tags",
                    action = "articles"
                }
            );

            config.Routes.MapHttpRoute(
                name: "CategoriesApi",
                routeTemplate: "api/categories/{categoryId}/articles",
                defaults: new
                {
                    controller = "categories",
                    action = "articles"
                }
            );


            config.Routes.MapHttpRoute(
                name: "ArticlesApi",
                routeTemplate: "api/articles/{articleId}/comment",
                defaults: new
                {
                    controller = "articles",
                    action = "comment"
                }

            );
             
            config.Routes.MapHttpRoute(
                name: "UsersApi",
                routeTemplate: "api/users/{action}",
                defaults: new
                {
                    controller = "users"
                }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
