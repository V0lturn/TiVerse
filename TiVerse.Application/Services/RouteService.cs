using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using TiVerse.Application.Data;
using TiVerse.Application.DTO;
using TiVerse.Application.Pagination;
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

        public async Task<List<Trip>> FindAllPossibleRoutes(string Departure, string Destination)
        {
            var routes = await _dbContext.Trips
                .Where(entity => entity.DeparturePoint.ToLower().Contains(Departure.ToLower())
                && entity.DestinationPoint.ToLower().Contains(Destination.ToLower()))
                .OrderBy(entity => entity.Date)
                .ToListAsync();

            return routes;
        }  

        public async Task<PagedList<Trip>> FindRouteByTransoprt(string TransportType, int page, int pageSize)
        {
            var tripQuery = _dbContext.Trips
                .Where(entity => entity.Transport.ToLower() == TransportType.ToLower());

            return await PagedList<Trip>.CreateAsync(tripQuery, page, pageSize);
        }

        public decimal GetMaxPriceInCategory(string TransportType)
        {
            return _dbContext.Trips
                .Where(entity => entity.Transport.ToLower() == TransportType.ToLower())
                .Max(entity => entity.TicketCost);
        }

        public async Task<List<string>> GetAllCities(string TransportType)
        {
            return await _dbContext.Trips
                .Where(entity => entity.Transport.ToLower() == TransportType.ToLower())
                .Select(entity => entity.DeparturePoint)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<DTO.TopRoutesDTO>> MostPopularRoutesInUkraine()
        {
            var query = (from trip in _dbContext.Trips
                         join depLocation in _dbContext.Locations on trip.DeparturePoint equals depLocation.City
                         join destLocation in _dbContext.Locations on trip.DestinationPoint equals destLocation.City
                         where depLocation.Country == "Україна" && destLocation.Country == "Україна"
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

        public static Expression<Func<Trip, object>> GetSortProperty(string sortColumn)
        {
            return sortColumn.ToLower() switch
            {
                "departurecity" => trip => trip.DeparturePoint,
                "destinationcity" => trip => trip.DestinationPoint,
                "date" => trip => trip.Date,
                "availableseats" => trip => trip.Places,
                "price" => trip => trip.TicketCost,
                _ => trip => trip.TripID
            }; ;
        }

        public async Task<PagedList<Trip>> LoadRoutesWithoutSorting(string selectedTransport, int page, int pageSize, int minPrice, int maxPrice)
        {
            IQueryable<Trip> query = _dbContext.Trips
                .Where(entity => entity.Transport.ToLower() == selectedTransport.ToLower()
                && entity.TicketCost >= minPrice && entity.TicketCost <= maxPrice);

            return await PagedList<Trip>.CreateAsync(query, page, pageSize);
        }

        public async Task<PagedList<Trip>> LoadRoutesWithSorting(string selectedTransport, string sortingCriteria, string sortOrder, 
            int page, int pageSize, int minPrice, int maxPrice, List<string> selectedCities)
        {
            IQueryable<Trip> query = _dbContext.Trips
                .Where(entity => entity.Transport.ToLower() == selectedTransport.ToLower()
                 && entity.TicketCost >= minPrice && entity.TicketCost <= maxPrice);

            if(selectedCities.Any())
            {
                query = query.Where(entity => selectedCities.Contains(entity.DeparturePoint));
            }

            if (sortOrder.ToLower() == "descending")
            {
                query = query.OrderByDescending(GetSortProperty(sortingCriteria));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(sortingCriteria));
            }

            return await PagedList<Trip>.CreateAsync(query, page, pageSize);
        }
    }
}
