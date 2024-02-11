using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TiVerse.Application.DTO;
using TiVerse.Application.Hubs;
using TiVerse.Application.Interfaces.IAccountServiceInterface;
using TiVerse.Application.Interfaces.IRouteServiceInterface;
using TiVerse.Application.ViewModels;
using TiVerse.WebUI.CityLocalizer;

namespace TiVerse.WebUI.Controllers
{
    [AllowAnonymous]
    public class MainPageController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRouteService _routeService;
        private readonly ICityLocalization _cityLocalization;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly HttpClient _httpClient;
        
        private readonly Uri ApiUrl = new Uri("https://localhost:6001/routes");

        public MainPageController(IMapper mapper, IRouteService routeService,
            ICityLocalization cityLocalization, IHubContext<MessageHub> hubContext)
        {
            _mapper = mapper;
            _routeService = routeService;
            _cityLocalization = cityLocalization;
            _httpClient = new HttpClient();
            _hubContext = hubContext;
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

        [HttpDelete]
        public async Task<IActionResult> DeleteRoute(Guid tripId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToPage("/CallApi");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _httpClient.DeleteAsync($"{ApiUrl}/{tripId}");

            if (response.IsSuccessStatusCode)
            {
                return Ok("Route deleted successfully");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound($"Route with ID {tripId} not found");
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"An error occurred: {response.ReasonPhrase}");
            }
        }

        public IActionResult RedirectToAddRoute()
        {
            return View("AddRoute");
        }

        [HttpPost]
        public async Task<IActionResult> AddRoute(RouteViewModel routeModel)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToPage("/CallApi");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var jsonContent = JsonConvert.SerializeObject(routeModel);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(ApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    await _hubContext.Clients.All.SendAsync("MessageCreated", routeModel);
                    TempData["SuccessMessage"] = "Нову поїздку додано успішно";
                    return View("AddRoute");
                }
                else
                {
                    TempData["ErrorMessage"] = "Помилка при додавнні нового маршруту";
                    return View("AddRoute");
                }
            }
        }
    }
}
