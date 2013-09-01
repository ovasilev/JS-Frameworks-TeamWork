using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        public string AuthCode { get; set; }

        public string SessionKey { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string DisplayName { get; set; }

        public bool IsAdmin { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Article> Articles { get; set; }

        public User()
        {
            this.Comments = new HashSet<Comment>();
            this.Articles = new HashSet<Article>();
        }
    }
}
