using Microsoft.EntityFrameworkCore;
using Vehicles.API.Data.Entities;

namespace Vehicles.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<VehicleType> VehicleTypes { get; set; }

        public DbSet<Procedure> Procedures { get; set; }

        public DbSet<DocumentType> DocumentTypes { get; set; }

        public DbSet<Brand> Brands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VehicleType>().HasIndex(x => x.Description).IsUnique();
            modelBuilder.Entity<Procedure>().HasIndex(x => x.Description).IsUnique();
            modelBuilder.Entity<DocumentType>().HasIndex(x => x.Description).IsUnique();
            modelBuilder.Entity<Brand>().HasIndex(x => x.Description).IsUnique();
        }

    }
}
