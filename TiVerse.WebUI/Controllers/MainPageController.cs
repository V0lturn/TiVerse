using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TiVerse.Application.DTO;
using TiVerse.Application.Interfaces.IRouteServiceInterface;

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
            return View("Index");
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


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var callbackUrl = Url.Action("Index", "MainPage");

            return SignOut(new AuthenticationProperties { RedirectUri = callbackUrl }, "Cookies", "oidc");
        }

        public IActionResult CallApi()
        {
            return RedirectToPage("/CallApi");
        }
    }
}
