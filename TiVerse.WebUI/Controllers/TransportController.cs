using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TiVerse.Application.DTO;
using TiVerse.Application.Interfaces.IRouteServiceInterface;
using TiVerse.Application.Pagination;
using TiVerse.Application.UseCase;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;

namespace TiVerse.WebUI.Controllers
{
    public class TransportController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRouteService _routeService;

        public TransportController(IMapper mapper, IRouteService routeService)
        {
            _mapper = mapper;
            _routeService = routeService;
        }

        public async Task<IActionResult> Index(string Transport)
        {
            var routes = await _routeService.FindRouteByTransoprt(Transport, 1, 50);

            ViewData["MaxPrice"] = _routeService.GetMaxPriceInCategory(Transport);
            ViewData["UniqueDeparturePoint"] = await _routeService.GetAllCities(Transport);

            if (!routes.Items.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            var routesToDTO = _mapper.Map<List<RouteDTO>>(routes.Items);
            return View(routesToDTO);
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
                loadedRoutes = await _routeService.LoadRoutesWithSorting(selectedTransport, sortingCriteria, sortOrder, page, pageSize, minPrice, maxPrice, selectedCities);
            }

            if (!loadedRoutes.Items.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            var routesToDTO = _mapper.Map<List<RouteDTO>>(loadedRoutes.Items);
            return PartialView("_DisplayRoutes", routesToDTO);
        }     
    }
}
