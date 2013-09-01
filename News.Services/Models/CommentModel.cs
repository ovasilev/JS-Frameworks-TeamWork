using System;
using System.Runtime.Serialization;

namespace News.Services.Models
{
    [DataContract()]
    public class CommentModel
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "commentedBy")]
        public string CommentedBy { get; set; }

        [DataMember(Name = "datePosted")]
        public DateTime DatePosted { get; set; }
    }
}