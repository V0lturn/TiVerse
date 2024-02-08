using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TiVerse.Application.DTO;
using TiVerse.Application.Interfaces.IAccountServiceInterface;
using TiVerse.Application.Interfaces.IRouteServiceInterface;
using TiVerse.WebUI.CityLocalizer;

namespace TiVerse.WebUI.Controllers
{
    [AllowAnonymous]
    public class MainPageController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRouteService _routeService;
        private readonly ICityLocalization _cityLocalization;

        public MainPageController(IMapper mapper, IRouteService routeService,
            ICityLocalization cityLocalization)
        {
            _mapper = mapper;
            _routeService = routeService;
            _cityLocalization = cityLocalization;
        }

        public async Task<IActionResult> Index()
        {
            var ukraine_routes = await _routeService.MostPopularRoutesInUkraine();
            ViewData["InUkraine"] = _cityLocalization.GetLocalizedList(ukraine_routes);

            var foreign_routes = await _routeService.MostPopularRoutesFromUkraine();
            ViewData["FromUkraine"] = _cityLocalization.GetLocalizedList(foreign_routes);

            string userId = HttpContext.User.FindFirst("sub")?.Value;
            ViewData["UserBalance"] = await _routeService.GetUserBalance(userId);

            return View("Index");
        }

        public async Task<IActionResult> FindSpecificRoute(string Departure, string Destination, DateTime Date, string Transport)
        {
            var routes = await _routeService.FindRouteWithParams(_cityLocalization.GetUkrainianCityName(Departure), _cityLocalization.GetUkrainianCityName(Destination), Date, Transport);

            if (!routes.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);
            var localizedRoutes = _cityLocalization.GetLocalizedList(routesToDTO);

            return View(localizedRoutes);
        }

        public async Task<IActionResult> FindAllRoutes(string Departure, string Destination)
        {
            var routes = await _routeService.FindAllPossibleRoutes(_cityLocalization.GetUkrainianCityName(Departure), _cityLocalization.GetUkrainianCityName(Destination));

            if (!routes.Any())
            {
                ViewData["Error"] = "Не знайдено квитків за заданим маршрутом";
            }

            var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);
            var localizedRoutes = _cityLocalization.GetLocalizedList(routesToDTO);

            return PartialView("_MostPopularRoutes", localizedRoutes);
        }


        [HttpGet]
        [Authorize(Policy = "RequireAuth")]
        public IActionResult Logout()
        {
            var callbackUrl = Url.Action("Index", "MainPage");

            return SignOut(new AuthenticationProperties { RedirectUri = callbackUrl }, "Cookies", "oidc");
        }

        [Authorize(Policy = "RequireAuth")]
        public IActionResult CallApi()
        {
            return RedirectToPage("/CallApi");
        }

        [HttpPost]
        [Route("/SetCulture")]
        public IActionResult SetCulture([FromQuery] string culture)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
            };

            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                cookieOptions);

            return Ok(new { success = true });
        }

    }
}
