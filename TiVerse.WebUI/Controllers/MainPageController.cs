using Microsoft.AspNetCore.Mvc;
namespace TiVerse.WebUI.Controllers
{
    public class MainPageController : Controller
    {
        public MainPageController() { }

        public IActionResult Index()
        {
            return View();
        }
    }
}
