using Microsoft.EntityFrameworkCore;
using TiVerse.Core.Entity;

namespace TiVerse.Infrastructure.AppDbContext
{
    public interface ITiVerseDbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<UserRouteHistory> UserRouteHistories { get; set; }
        public DbSet<Location> Locations { get; set; }
    }
}
