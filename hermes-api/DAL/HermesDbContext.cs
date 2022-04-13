using Microsoft.EntityFrameworkCore;

namespace hermes_api.DAL
{
    public class HermesDbContext : DbContext
    {
        public HermesDbContext(DbContextOptions<HermesDbContext> options) : base(options)
        {
        }

        public DbSet<FeedDALModel> Feed { get; set; }
        public DbSet<LogDALModel> Log { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedDALModel>().ToTable("Feed");
            modelBuilder.Entity<LogDALModel>().ToTable("Log");
        }
    }
}
