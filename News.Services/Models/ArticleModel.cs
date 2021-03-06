﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace News.Services.Models
{
    [DataContract()]
    public class ArticleModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "contentPreview")]
        public string ContentPreview { get; set; }

        [DataMember(Name = "datePublished")]
        public DateTime DatePublished { get; set; }

        [DataMember(Name = "author")]
        public string Author { get; set; }

        [DataMember(Name = "category")]
        public CategoryModel Category { get; set; }

        [DataMember(Name = "thumbUrl")]
        public string ThumbUrl { get; set; }

        [DataMember(Name = "readCount")]
        public int ReadCount { get; set; }

        [DataMember(Name = "tags")]
        public IEnumerable<string> Tags { get; set; }
    }
}