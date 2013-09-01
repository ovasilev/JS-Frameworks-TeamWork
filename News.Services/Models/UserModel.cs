using System;
using System.Linq;
using System.Runtime.Serialization;

namespace News.Services.Models
{
    [DataContract()]
    public class UserModel
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "displayName")]
        public string Displayname { get; set; }

        [DataMember(Name = "authCode")]
        public string AuthCode { get; set; }
    }
}