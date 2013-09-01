using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(40)]
        public string Name { get; set; }

        public virtual ICollection<Article> Articles { get; set; }

        public Category()
        {
            this.Articles = new HashSet<Article>();
        }
    }
}
