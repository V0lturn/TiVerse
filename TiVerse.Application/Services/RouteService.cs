using Microsoft.EntityFrameworkCore;
using TiVerse.Application.Data;
using TiVerse.Application.DTO;
using TiVerse.Core.Entity;

namespace TiVerse.Application.UseCase
{
    public class RouteService
    {
        private readonly IApplicationDbContext _dbContext;

        public RouteService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Trip>> FindRouteWithParams(string Departure, string Destination, DateTime Date, string Transport)
        {
            var query = _dbContext.Trips
                .Where(entity => entity.DeparturePoint.ToLower().Contains(Departure.ToLower())
                && entity.DestinationPoint.ToLower().Contains(Destination.ToLower())
                && entity.Date.Date == Date.Date);

            if (!string.IsNullOrWhiteSpace(Transport))
            {
                query = query.Where(entity => entity.Transport.ToLower() == Transport.ToLower());
            }

            var routes = await query.ToListAsync();

            return routes;
        }

        public async Task<List<Trip>> FindAllPossibleRoutes (string Departure, string Destination)
        {
            var routes = await _dbContext.Trips
                .Where(entity => entity.DeparturePoint.ToLower().Contains(Departure.ToLower())
                && entity.DestinationPoint.ToLower().Contains(Destination.ToLower()))
                .ToListAsync();

            return routes;
        }

        public async Task<List<Trip>> FindRouteByTransoprt(string TransoprtType)
        {

            var routes = await _dbContext.Trips
              .Where(entity => entity.Transport.ToLower() == TransoprtType.ToLower())
              .Take(50)
              .ToListAsync();

            return routes;
        }


        public async Task<List<DTO.TopRoutesDTO>> MostPopularRoutesInUkraine()
        {
            var topRoutes = await _dbContext.Trips
                .GroupBy(t => new { t.DeparturePoint, t.DestinationPoint })
                .Select(group => new DTO.TopRoutesDTO
                {
                    DeparturePoint = group.Key.DeparturePoint,
                    DestinationPoint = group.Key.DestinationPoint,
                    RouteFrequency = group.Count(),
                    MinPrice = group.Min(t => t.TicketCost)
                })
                .OrderByDescending(result => result.RouteFrequency)
                .Take(5)
                .ToListAsync();

            return topRoutes;
        }

        public async Task<List<DTO.TopRoutesDTO>> MostPopularRoutesFromUkraine()
        {
            var query = (from trip in _dbContext.Trips
                         join depLocation in _dbContext.Locations on trip.DeparturePoint equals depLocation.City
                         join destLocation in _dbContext.Locations on trip.DestinationPoint equals destLocation.City
                         where depLocation.Country == "Україна" && destLocation.Country != "Україна"
                         group new { trip.DeparturePoint, trip.DestinationPoint } by new { trip.DeparturePoint, trip.DestinationPoint } into grouped
                         select new TopRoutesDTO
                         {
                             DeparturePoint = grouped.Key.DeparturePoint,
                             DestinationPoint = grouped.Key.DestinationPoint,
                             RouteFrequency = grouped.Count(),
                             MinPrice = (from t in _dbContext.Trips
                                         where t.DeparturePoint == grouped.Key.DeparturePoint && t.DestinationPoint == grouped.Key.DestinationPoint
                                         select t.TicketCost).Min()
                         })
                    .OrderByDescending(result => result.RouteFrequency)
                    .Take(5);


            var routes = await query.ToListAsync();

            return routes;
          
        }
    }
}
