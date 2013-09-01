namespace News.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using News.Models;

    public sealed class Configuration : DbMigrationsConfiguration<News.Data.NewsContext>
    {
        private NewsContext context;

        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
            this.context = new NewsContext();
            //this.Seed(this.context);
        }

        protected override void Seed(News.Data.NewsContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            User admin = new User()
            {
                Username = "Administrator",
                DisplayName = "Site administrator",
                AuthCode = "d033e22ae348aeb5660fc2140aec35850c4da997",
                IsAdmin = true,
            };

            context.Users.AddOrUpdate(admin);
            context.SaveChanges();
        }
    }
}
