using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TiVerse.WebUI.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoadPartialView(string viewName)
        {
            return viewName switch
            {
                "_PersonalInfo" => PartialView("_PersonalInfo"),
                "_PlannedTrip" => PartialView("_PlannedTrip"),
                "_TripHistory" => PartialView("_TripHistory"),
                _ => new EmptyResult(),
            };
        }  
    }
}
