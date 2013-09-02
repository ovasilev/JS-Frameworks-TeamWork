using News.Data;
using News.Models;
using News.Services.Models;
using News.Services.Atributes;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ValueProviders;

namespace News.Services.Controllers
{
    public class UsersController : BaseApiController
    {
        private const int MinUserNameLenght = 6;
        private const int MaxUserNameLenght = 30;
        private const int MinNicknameLenght = 6;
        private const int MaxNicknameLenght = 30;
        private const string ValidUserNameCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890_.";
        private const string ValidNickNameCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890_. -";
        private const string SessionKeyChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
        private static readonly Random rand = new Random();

        private const int Sha1Length = 40;
        private const int SessionKeyLenght = 50;

        public UsersController()
            : base()
        {
        }

        public UsersController(IDbContextFactory<DbContext> contextFactory)
            : base(contextFactory)
        {
        }

        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage PostRegisterUser(UserModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                 () =>
                 {
                     var context = this.contextFactory.Create() as NewsContext;

                     using (context)
                     {
                         this.ValidateUserName(model.Username);
                         this.ValidateNickname(model.Displayname);
                         this.ValidateAuthCode(model.AuthCode);
                         var usernameToLower = model.Username.ToLower();
                         var displaynameToLower = model.Displayname.ToLower();
                         var user = context.Users.FirstOrDefault(usr => usr.Username == usernameToLower || usr.DisplayName.ToLower() == displaynameToLower);

                         if (user != null)
                         {
                             throw new InvalidOperationException("User already exists in the database!");
                         }

                         user = new User()
                         {
                             Username = usernameToLower,
                             DisplayName = model.Displayname,
                             AuthCode = model.AuthCode,
                             IsAdmin = model.IsAdmin,
                         };

                         context.Users.Add(user);
                         context.SaveChanges();

                         user.SessionKey = this.GenerateSessionKey(user.Id);
                         context.SaveChanges();

                         var loggedModel = new UserLoggedModel()
                         {
                             Displayname = user.DisplayName,
                             SessionKey = user.SessionKey,
                             IsAdmin = user.IsAdmin,
                         };

                         var responce = this.Request.CreateResponse(HttpStatusCode.Created, loggedModel);

                         return responce;
                     }
                 });

            return responseMsg;
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage PostLoginUser(UserModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                  () =>
                  {
                      var context = this.contextFactory.Create() as NewsContext;

                      using (context)
                      {
                          this.ValidateUserName(model.Username);
                          this.ValidateAuthCode(model.AuthCode);
                          var usernameToLower = model.Username.ToLower();
                          var user = context.Users.FirstOrDefault(usr => usr.Username == usernameToLower && usr.AuthCode == model.AuthCode);

                          if (user == null)
                          {
                              throw new InvalidOperationException("Invalid username or password!");
                          }
                          if (user.SessionKey == null)
                          {
                              user.SessionKey = this.GenerateSessionKey(user.Id);
                              context.SaveChanges();
                          }

                          var loggedModel = new UserLoggedModel()
                          {
                              Displayname = user.DisplayName,
                              SessionKey = user.SessionKey,
                              IsAdmin = user.IsAdmin,
                          };

                          var responce = this.Request.CreateResponse(HttpStatusCode.Created, loggedModel);

                          return responce;
                      }
                  });

            return responseMsg;
        }

        [HttpPut]
        [ActionName("logout")]
        public HttpResponseMessage PutLogoutUser([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                 () =>
                 {
                     var context = this.contextFactory.Create() as NewsContext;

                     using (context)
                     {
                         this.ValidateSessionKey(sessionKey);
                         var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                         if (user == null)
                         {
                             throw new InvalidOperationException("Invalid SessionKey!");
                         }

                         user.SessionKey = null;
                         context.SaveChanges();

                         var responce = this.Request.CreateResponse(HttpStatusCode.OK);

                         return responce;
                     }
                 });

            return responseMsg;
        }

        private string GenerateSessionKey(int userId)
        {
            var skeyBuilder = new StringBuilder(SessionKeyLenght);
            skeyBuilder.Append(userId);

            while (skeyBuilder.Length < SessionKeyLenght)
            {
                var index = rand.Next(SessionKeyChars.Length);
                skeyBuilder.Append(SessionKeyChars[index]);
            }

            return skeyBuilder.ToString();
        }

        private void ValidateAuthCode(string authCode)
        {
            if (authCode == null)
            {
                throw new ArgumentNullException("Authentication code cannot be null(empty)!");
            }
            else if (authCode.Length != Sha1Length)
            {
                throw new ArgumentOutOfRangeException("Password must be properlly encypted!");
            }
        }

        private void ValidateNickname(string nickName)
        {
            if (nickName == null)
            {
                throw new ArgumentNullException("Username cannot be null!");
            }
            else if (nickName.Length < MinUserNameLenght)
            {
                throw new ArgumentOutOfRangeException(string.Format("Username lenght must be at least {0} characters!", MinNicknameLenght));
            }
            else if (nickName.Length > MaxUserNameLenght)
            {
                throw new ArgumentOutOfRangeException(string.Format("Username lenght must be less than {0} characters!", MaxNicknameLenght));
            }
            else if (nickName.Any(ch => !ValidNickNameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(string.Format("Username must contain only Lattin letters, digits, '.' , ',', ' ' and '-'"));
            }
        }

        private void ValidateUserName(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("Username cannot be null!");
            }
            else if (userName.Length < MinUserNameLenght)
            {
                throw new ArgumentOutOfRangeException(string.Format("Username lenght must be at least {0} characters!", MinUserNameLenght));
            }
            else if (userName.Length > MaxUserNameLenght)
            {
                throw new ArgumentOutOfRangeException(string.Format("Username lenght must be less than {0} characters!", MaxUserNameLenght));
            }
            else if (userName.Any(ch => !ValidUserNameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(string.Format("Username must contain only Lattin letters, digits, '.' and ','"));
            }
        }
    }
}
