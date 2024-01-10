using TiVerse.Application.DTO;
using TiVerse.Application.Pagination;
using TiVerse.Core.Entity;

namespace TiVerse.Application.Interfaces.IRouteServiceInterface
{
    public interface IRouteService
    {
        Task<List<Trip>> FindRouteWithParams(string Departure, string Destination, DateTime Date, string Transport);
        Task<List<Trip>> FindAllPossibleRoutes(string Departure, string Destination);
        Task<PagedList<Trip>> FindRouteByTransoprt(string TransportType, int page, int pageSize);
        decimal GetMaxPriceInCategory(string TransportType);
        Task<List<string>> GetAllCities(string TransportType);
        Task<List<TopRoutesDTO>> MostPopularRoutesInUkraine();
        Task<List<TopRoutesDTO>> MostPopularRoutesFromUkraine();
        Task<PagedList<Trip>> LoadRoutesWithoutSorting(string selectedTransport, int page, int pageSize, int minPrice, int maxPrice);
        Task<PagedList<Trip>> LoadRoutesWithSorting(string selectedTransport, string sortingCriteria, string sortOrder, int page, int pageSize, int minPrice, int maxPrice, List<string> selectedCities);
    }
}
