using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using News.Data;
using News.Services.Models;

namespace News.Services.Controllers
{
    public class BaseApiController : ApiController
    {
        private const int SessionKeyLenght = 50;

        protected readonly IDbContextFactory<DbContext> contextFactory;

        public BaseApiController()
        {
            this.contextFactory = new NewsContextFactory();
        }

        public BaseApiController(IDbContextFactory<DbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        protected T PerformOperationAndHandleExceptions<T>(Func<T> operation)
        {
            try
            {
                return operation();
            }

            catch (Exception ex)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                throw new HttpResponseException(errResponse);
            }
        }

        protected void ValidateSessionKey(string sessionKey)
        {
            var context = this.contextFactory.Create() as NewsContext;

            if (sessionKey == null)
            {
                throw new ArgumentNullException("SessionKey cannot be null(empty)!");
            }
            else if (sessionKey.Length != SessionKeyLenght)
            {
                throw new ArgumentOutOfRangeException("Invalid Sessionkey!");
            }

            var user = context.Users.Where(u => u.SessionKey == sessionKey).FirstOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException("Such user does not exists!");
            }
        }
    }
}
