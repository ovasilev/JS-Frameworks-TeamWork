using News.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace News.Services.Models
{
    public class NewsContextFactory : IDbContextFactory<DbContext>
    {
        public DbContext Create()
        {
            return new NewsContext();
        }
    }
}