using GarbageCollectionApp.Models;
using Microsoft.EntityFrameworkCore;
namespace GarbageCollectionApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<GarbageCollection> GarbageCollections { get; set; }
    }
}

