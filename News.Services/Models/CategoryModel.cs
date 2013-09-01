using System.Runtime.Serialization;

namespace News.Services.Models
{
    [DataContract()]
    public class CategoryModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}