using System.Runtime.Serialization;

namespace News.Services.Models
{
    [DataContract()]
    public class TagModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "posts")]
        public int Count { get; set; }
    }
}