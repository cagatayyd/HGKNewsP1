using Microsoft.EntityFrameworkCore;
using HGKNews.Entities;

namespace HGKNews.Context
{
    public class NewsDbContext : DbContext
    {
        public NewsDbContext()
        {

        }

        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options)
        {

        }

        public DbSet<NewsItem> NewsItems { get; set; }

    }
}
