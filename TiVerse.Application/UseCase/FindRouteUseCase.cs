using Microsoft.EntityFrameworkCore;
using TiVerse.Application.Data;
using TiVerse.Core.Entity;
namespace TiVerse.Application.UseCase
{
    public class FindRouteUseCase
    {
        private readonly IApplicationDbContext _dbContext;

        public FindRouteUseCase(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Trip>> Routes(string Departure, string Destination, DateTime Date, string Transport)
        {
            var query = _dbContext.Trips
                .Where(entity => entity.DeparturePoint.Contains(Departure)
                && entity.DestinationPoint.Contains(Destination)
                && entity.Date.Date == Date.Date);

            if (!string.IsNullOrWhiteSpace(Transport))
            {
                query = query.Where(entity => entity.Transport.ToLower() == Transport);
            }

            var routes = await query.ToListAsync();

            return routes;
        }
    }
}
