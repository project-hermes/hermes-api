using Microsoft.EntityFrameworkCore;

namespace hermes_api.DAL
{
    public class HermesDbContext : DbContext
    {
        //Add-Migration XXXX
        //Update-Database

        public HermesDbContext(DbContextOptions<HermesDbContext> options) : base(options)
        {
        }

        public DbSet<RemoraDALModel> Remora { get; set; }
        public DbSet<RemoraRecordDALModel> RemoraRecord { get; set; }
        public DbSet<FirmwareDALModel> Firmware { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RemoraDALModel>().ToTable("Remora");
            modelBuilder.Entity<RemoraRecordDALModel>().ToTable("RemoraRecord");
            modelBuilder.Entity<FirmwareDALModel>().ToTable("Firmware");
        }
    }
}
