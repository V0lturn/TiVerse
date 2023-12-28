using Microsoft.AspNetCore.Mvc;

namespace TiVerse.WebUI.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
