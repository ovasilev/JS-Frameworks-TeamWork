using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace News.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required]
        [MinLength(50)]
        public string Content { get; set; }

        [Required]
        public DateTime DatePublished { get; set; }

        public int ReadCount { get; set; }

        [Required]
        public virtual User Author { get; set; }


        public virtual Image Image { get; set; }

        [Required]
        public virtual Category Category { get; set; }
         
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public Article()
        {
            this.Tags = new HashSet<Tag>();
            this.Comments = new HashSet<Comment>();
        }
    }
}
