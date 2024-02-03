using Microsoft.EntityFrameworkCore;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.DbInitialize;
namespace TiVerse.Infrastructure.AppDbContext
{
    public class TiVerseDbContext : DbContext, ITiVerseDbContext
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
            modelBuilder.Entity<Account>()
               .Property(a => a.CashBalance)
               .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Trip>()
               .Property(a => a.TicketCost)
               .HasColumnType("decimal(18,2)");

            SeedDb.SeedTrips(modelBuilder);
            SeedDb.SeedLocations(modelBuilder);
        }
    }
}