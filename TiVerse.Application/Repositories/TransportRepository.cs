using Microsoft.EntityFrameworkCore;
using TiVerse.Application.Interfaces.ITransportRepositoryInterface;
using TiVerse.Application.ViewModels;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;

namespace TiVerse.Application.Repositories
{
    public class TransportRepository : ITransportRepository
    {
        private readonly TiVerseDbContext _dbContext;

        public TransportRepository(TiVerseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Trip> GetAllRoutes()
        {
            return _dbContext.Set<Trip>();
        }

        public IQueryable<Trip> GetSomeRoutes(int quantity = 50)
        {
            return _dbContext.Set<Trip>().Take(quantity);
        }

        public IQueryable<Trip> GetRoutesByTransport(string transport)
        {
            var allRoutesByTransort = _dbContext.Trips.Where(r => r.Transport == transport);

            return allRoutesByTransort;
        }

        public bool CreateRoute(RouteViewModel viewModel)
        {
            if(viewModel.TicketCost <= 0 || viewModel.Places <= 0 || viewModel.DateOfTrip < DateTime.Now)
            {
                return false;
            }


            var newRoute = new Trip(
                viewModel.DeparturePoint,
                viewModel.DestinationPoint,
                viewModel.DateOfTrip,
                viewModel.Transport,
                viewModel.Company,
                viewModel.Places,
                viewModel.TicketCost
            );

            _dbContext.Trips.Add(newRoute);
            _dbContext.SaveChanges();

            return true;           
        }

        public async Task<bool> UpdateRoute(Guid id, RouteViewModel viewModel)
        {
            var existingRoute = await _dbContext.Trips.FirstOrDefaultAsync(r => r.TripID == id);
            if (existingRoute == null)
                return false;

            if (viewModel.DeparturePoint != null)
            {
                existingRoute.DeparturePoint = viewModel.DeparturePoint;
            }
            
            if (viewModel.DestinationPoint != null)
            {
                existingRoute.DestinationPoint = viewModel.DestinationPoint;
            }

            if(viewModel.DateOfTrip > DateTime.Now)
            {
                existingRoute.Date = viewModel.DateOfTrip;
            }

            if (viewModel.Company != null)
            {
                existingRoute.Company = viewModel.Company;
            }

            if (viewModel.Places > 0)
            {
                existingRoute.DestinationPoint = viewModel.DestinationPoint;
            }

            if (viewModel.TicketCost > 0)
            {
                existingRoute.TicketCost = viewModel.TicketCost;
            }

            existingRoute.Transport = viewModel.Transport;

            return true;
        }

        public async Task<bool> DeleteRoute(Guid id)
        {
            var route = await _dbContext.Trips.FirstOrDefaultAsync(r => r.TripID == id);
            if (route == null)
                return false;

            _dbContext.Trips.Remove(route);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
