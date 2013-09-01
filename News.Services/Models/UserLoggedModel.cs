using System.Runtime.Serialization;

namespace News.Services.Models
{
    [DataContract()]
    public class UserLoggedModel
    {
        [DataMember(Name = "displayName")]
        public string Displayname { get; set; }

        [DataMember(Name = "sessionKey")]
        public string SessionKey { get; set; }
    }
}