using GarbageCollectionApp.Models;
using Microsoft.EntityFrameworkCore;
namespace GarbageCollectionApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<GarbageCollection> GarbageCollections { get; set; }
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<GarbageBin> GarbageBins { get; set; }
        public DbSet<GarbageBinCitizen> GarbageBinCitizens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GarbageBinCitizen>()
                .HasKey(gbc => new { gbc.IdGarbageBin, gbc.IdCitizen });
        }
    }
}

