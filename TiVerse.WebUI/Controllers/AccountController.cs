using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiVerse.Application.Interfaces.IAccountServiceInterface;
using TiVerse.Application.Interfaces.IRepositoryInterface;
using TiVerse.Application.Interfaces.IRouteServiceInterface;
using TiVerse.Application.UseCase;
using TiVerse.Core.Entity;
using TiVerse.WebUI.ViewModels;

namespace TiVerse.WebUI.Controllers
{
    [Authorize(Policy = "RequireAuth")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IRouteService _routeService;
        private readonly ITiVerseIRepository<Account> _accountRepository;

        
        public AccountController(IAccountService accountService, ITiVerseIRepository<Account> accountRepository,
            IRouteService routeService)
        {
            _accountService = accountService;
            _accountRepository = accountRepository;
            _routeService = routeService;
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
                ViewData["PlannedTrip"] = await _accountService.GetUserPlannedTrips(userId);
            }

            if (viewName == "_TripHistory")
            {
                ViewData["TripHistory"] = await _accountService.GetUserTripHistory(userId);
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
