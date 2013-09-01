using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using News.Data;
using News.Models;
using System.Data.Entity;
using News.Data.Migrations;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<NewsContext, Configuration>());

            var context = new NewsContext();

            var users = context.Users.FirstOrDefault();
            if (users != null)
            {
                Console.WriteLine(users.DisplayName);
            }

        }
    }
}
