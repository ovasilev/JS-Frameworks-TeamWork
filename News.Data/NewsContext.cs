using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using News.Models;

namespace News.Data
{
    public class NewsContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Image> Images { get; set; }

        public NewsContext()
            : base("NewsDb")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(usr => usr.AuthCode).IsFixedLength().HasMaxLength(40);
            modelBuilder.Entity<User>().Property(usr => usr.SessionKey).IsFixedLength().HasMaxLength(50);

            base.OnModelCreating(modelBuilder);
        }
    }
}
