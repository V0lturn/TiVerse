using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;

namespace TiVerse.WebUI.Controllers
{
    public class PlaneController : Controller
    {
        private readonly TiVerseDbContext _context;

        public PlaneController(TiVerseDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Trip> busModel = new List<Trip>();
            busModel = await _context.Trips.Where(t => t.Transport == "Plane").Take(50).ToListAsync();
            return View("Index", busModel);
        }
    }
}
