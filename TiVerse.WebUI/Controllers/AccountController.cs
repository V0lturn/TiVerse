using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TiVerse.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoadPartialView(string viewName)
        {
            //ViewBag["UserID"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return viewName switch
            {
                "_PersonalInfo" => PartialView("_PersonalInfo"),
                "_PlannedTrip" => PartialView("_PlannedTrip"),
                "_TripHistory" => PartialView("_TripHistory"),
                _ => new EmptyResult(),
            };
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "MainPage"); 
        }
    }
}
