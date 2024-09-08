using AdSetIntegrador.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdSetIntegrador.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<OptionalFeature> OptionalFeatures { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<VehicleOptionalFeature> VehicleOptionalFeatures { get; set; }
    }
}
