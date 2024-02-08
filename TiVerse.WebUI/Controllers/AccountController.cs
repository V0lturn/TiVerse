using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using TiVerse.Application.DTO;
using TiVerse.Application.Interfaces.IAccountServiceInterface;
using TiVerse.Application.Interfaces.IRepositoryInterface;
using TiVerse.Application.Interfaces.IRouteServiceInterface;
using TiVerse.Application.UseCase;
using TiVerse.Core.Entity;
using TiVerse.WebUI.CityLocalizer;
using TiVerse.WebUI.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TiVerse.WebUI.Controllers
{
    [Authorize(Policy = "RequireAuth")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IRouteService _routeService;
        private readonly ITiVerseIRepository<Account> _accountRepository;
        private readonly IMapper _mapper;
        private readonly ICityLocalization _cityLocalization;

        public AccountController(IAccountService accountService, ITiVerseIRepository<Account> accountRepository,
            IRouteService routeService, IMapper mapper, ICityLocalization cityLocalization)
        {
            _accountService = accountService;
            _accountRepository = accountRepository;
            _routeService = routeService;
            _mapper = mapper;
            _cityLocalization = cityLocalization;
        }

        public async Task<IActionResult> IndexAsync()
        {
            string userId = HttpContext.User.FindFirst("sub")?.Value;
            ViewData["UserBalance"] = await _routeService.GetUserBalance(userId);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadPartialViewAsync(string viewName)
        {
            string userId = HttpContext.User.FindFirst("sub")?.Value;

            if (viewName == "_PersonalInfo")
            {
                var existingUser = _accountRepository.GetAll<Account>().FirstOrDefault(u => u.UserId == userId);
                ViewData["ExistingUserData"] = existingUser;
            }

            if (viewName == "_PlannedTrip")
            {
                var routes = await _accountService.GetUserPlannedTrips(userId);
                var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);

                var localizedRoutes = _cityLocalization.GetLocalizedList(routesToDTO);

                ViewData["PlannedTrip"] = localizedRoutes;
            }

            if (viewName == "_TripHistory")
            {
                var routes = await _accountService.GetUserTripHistory(userId);
                var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);

                var localizedRoutes = _cityLocalization.GetLocalizedList(routesToDTO);

                ViewData["TripHistory"] = localizedRoutes;
            }

            if (viewName == "_AccountBalance")
            {
                ViewData["AccountBalance"] = await _routeService.GetUserBalance(userId);
            }

            return viewName switch
            {
                "_PersonalInfo" => PartialView("_PersonalInfo"),
                "_PlannedTrip" => PartialView("_PlannedTrip"),
                "_TripHistory" => PartialView("_TripHistory"),
                "_AccountBalance" => PartialView("_AccountBalance"),
                _ => new EmptyResult(),
            };
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePersonalInfo([FromForm] PersonalInfoViewModel model)
        {
            string userId = HttpContext.User.FindFirst("sub")?.Value;

            var result = await _accountService.UpdateAccountInfo(model, userId);

            if (result.success)
            {
                TempData["SuccessMessage"] = result.message;
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> BuyTicket(Guid tripId)
        {
            TempData["SuccessMessage"] = string.Empty;
            TempData["ErrorMessage"] = string.Empty;

            string userId = HttpContext.User.FindFirst("sub")?.Value;

            var result = await _accountService.BuyTicket(tripId, userId);

            if (result.success)
            {
                TempData["SuccessMessage"] = result.message;
                return Json(new { success = true, message = result.message });
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
                return Json(new { success = false, message = result.message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TopUpBalance(decimal topUpAmount)
        {
            string userId = HttpContext.User.FindFirst("sub")?.Value;

            var result = await _accountService.UpdateUserBalance(topUpAmount, userId);

            if (result.success)
            {
                TempData["SuccessMessage"] = result.message;
            }
            else
            {
                TempData["ErrorMessage"] = result.message;

            }
            return RedirectToAction("Index"); 
        }

    }
}
