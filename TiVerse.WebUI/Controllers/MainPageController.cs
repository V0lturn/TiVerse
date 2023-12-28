using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TiVerse.Application.DTO;
using TiVerse.Application.UseCase;
using TiVerse.Infrastructure.AppDbContext;

namespace TiVerse.WebUI.Controllers
{
    public class MainPageController : Controller
    {
        private readonly TiVerseDbContext _context;
        private readonly IMapper _mapper;

        public MainPageController(TiVerseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var routeService = new RouteService(_context);

            var ukraine_routes = await routeService.MostPopularRoutesInUkraine();
            ViewData["InUkraine"] = ukraine_routes;

            var foreign_routes = await routeService.MostPopularRoutesFromUkraine();
            ViewData["FromUkraine"] = foreign_routes;
            return View();
        }


        public async Task<IActionResult> FindSpecificRoute(string Departure, string Destination, DateTime Date, string Transport)
        {
            var findRoutes = new RouteService(_context);
            var routes = await findRoutes.FindRouteWithParams(Departure, Destination, Date, Transport);

            if (!routes.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            return View("FindedRoutes", routes);
        }

        public async Task<IActionResult> FindAllRoutes(string Departure, string Destination)
        {
            var findRoutes = new RouteService(_context);
            var routes = await findRoutes.FindAllPossibleRoutes(Departure, Destination);

            if (!routes.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);
            return PartialView("_DisplayRoutes", routesToDTO);
        }

        public async Task<IActionResult> FindRoutesByTransport(string Transport)
        {
            var findRoutes = new RouteService(_context);
            var routes = await findRoutes.FindRouteByTransoprt(Transport);

            if (!routes.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);
            return PartialView("_DisplayRoutes", routesToDTO); 
        }

    }
}
