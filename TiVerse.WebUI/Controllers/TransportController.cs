using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TiVerse.Application.DTO;
using TiVerse.Application.Pagination;
using TiVerse.Application.UseCase;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;

namespace TiVerse.WebUI.Controllers
{
    public class TransportController : Controller
    {
        private readonly TiVerseDbContext _context;
        private readonly IMapper _mapper;

        public TransportController(TiVerseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string Transport)
        {
            var routeService = new RouteService(_context);
            var routes = await routeService.FindRouteByTransoprt(Transport, 1, 50);

            ViewData["MaxPrice"] = routeService.GetMaxPriceInCategory(Transport);
            ViewData["UniqueDeparturePoint"] = await routeService.GetAllCities(Transport);

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
            var routeService = new RouteService(_context);
            PagedList<Trip> loadedRoutes;

            if (sortingCriteria == null)
            {
                loadedRoutes = await routeService.LoadRoutesWithoutSorting(selectedTransport, page, pageSize, minPrice, maxPrice);
            }
            else
            {
                loadedRoutes = await routeService.LoadRoutesWithSorting(selectedTransport, sortingCriteria, sortOrder, page, pageSize, minPrice, maxPrice, selectedCities);
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
