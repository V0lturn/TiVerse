using Microsoft.EntityFrameworkCore;
using TiVerse.Application.Data;
using TiVerse.Application.SeedDB;
using TiVerse.Core.Entity;
namespace TiVerse.Infrastructure.AppDbContext
{
    public class TiVerseDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<UserRouteHistory> UserRouteHistories { get; set; }
        public DbSet<Location> Locations { get; set; }

        public TiVerseDbContext(DbContextOptions<TiVerseDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedDb.SeedTrips(modelBuilder);
            SeedDb.SeedLocations(modelBuilder);
        }
    }
}
