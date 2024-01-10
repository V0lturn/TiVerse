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
    public class MainPageController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRouteService _routeService;

        public MainPageController(IMapper mapper, IRouteService routeService)
        {
            _mapper = mapper;
            _routeService = routeService;
        }

        public async Task<IActionResult> Index()
        {
            var ukraine_routes = await _routeService.MostPopularRoutesInUkraine();
            ViewData["InUkraine"] = ukraine_routes;

            var foreign_routes = await _routeService.MostPopularRoutesFromUkraine();
            ViewData["FromUkraine"] = foreign_routes;
            return View();
        }

        public async Task<IActionResult> FindSpecificRoute(string Departure, string Destination, DateTime Date, string Transport)
        {        
            var routes = await _routeService.FindRouteWithParams(Departure, Destination, Date, Transport);

            if (!routes.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);
            return PartialView("_MostPopularRoutes", routesToDTO);
        }

        public async Task<IActionResult> FindAllRoutes(string Departure, string Destination)
        {
            var routes = await _routeService.FindAllPossibleRoutes(Departure, Destination);

            if (!routes.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);
            return PartialView("_MostPopularRoutes", routesToDTO);
        }     
    }
}
