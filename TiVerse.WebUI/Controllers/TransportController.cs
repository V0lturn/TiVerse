using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TiVerse.Application.DTO;
using TiVerse.Application.Interfaces.IRouteServiceInterface;
using TiVerse.Application.Pagination;
using TiVerse.Application.UseCase;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;
using TiVerse.WebUI.CityLocalizer;

namespace TiVerse.WebUI.Controllers
{
    public class TransportController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRouteService _routeService;
        private readonly ICityLocalization _cityLocalization;

        const int firstPage = 1;
        const int defaultPageSize = 50;

        public TransportController(IMapper mapper, IRouteService routeService, 
            ICityLocalization cityLocalization)
        {
            _mapper = mapper;
            _routeService = routeService;
            _cityLocalization = cityLocalization;
        }

        public async Task<IActionResult> Index(string Transport)
        {
            TempData["SuccessMessage"] = string.Empty;
            TempData["ErrorMessage"] = string.Empty;

            var routes = await _routeService.FindRouteByTransoprt(Transport, firstPage, defaultPageSize);

            ViewData["MaxPrice"] = _routeService.GetMaxPriceInCategory(Transport);
            
            var uniqueCities = await _routeService.GetAllCities(Transport);
            var localizedCities = _cityLocalization.GetLocalizedList(uniqueCities);

            ViewData["UniqueDeparturePoint"] = localizedCities;

            if (!routes.Items.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            string userId = HttpContext.User.FindFirst("sub")?.Value;
            ViewData["UserBalance"] = await _routeService.GetUserBalance(userId);

            var routesToDTO = _mapper.Map<List<RouteDTO>>(routes.Items);
            var localizedRoutes = _cityLocalization.GetLocalizedList(routesToDTO);

            return View(localizedRoutes);
        }

        [HttpPost]
        public async Task<IActionResult> LoadRoutes(string selectedTransport, string sortingCriteria, string sortOrder, int minPrice, int maxPrice, 
            int page, List<string> selectedCities, int pageSize = 50)
        {
            PagedList<Trip> loadedRoutes;

            if (sortingCriteria == null)
            {
                loadedRoutes = await _routeService.LoadRoutesWithoutSorting(selectedTransport, page, pageSize, minPrice, maxPrice);
            }
            else
            {
                List<string> localizedList = new List<string>();
                foreach(var item in selectedCities)
                {
                    var localizedCity = _cityLocalization.GetUkrainianCityName(item);
                    localizedList.Add(localizedCity);
                }

                loadedRoutes = await _routeService.LoadRoutesWithSorting(selectedTransport, sortingCriteria, sortOrder, page, pageSize, minPrice, maxPrice, localizedList);
            }

            if (!loadedRoutes.Items.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            var routesToDTO = _mapper.Map<List<RouteDTO>>(loadedRoutes.Items);
            var localizedRoutes = _cityLocalization.GetLocalizedList(routesToDTO);

            return PartialView("_DisplayRoutes", localizedRoutes);
        }     
    }
}
