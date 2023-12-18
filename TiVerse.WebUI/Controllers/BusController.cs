using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;

namespace TiVerse.WebUI.Controllers
{
    public class BusController : Controller
    {
        private readonly TiVerseDbContext _context;

        public BusController(TiVerseDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index() 
        {
            List<Trip> busModel = new List<Trip>();
            busModel = await _context.Trips.Where(t => t.Transport == "Bus").Take(50).ToListAsync();
            return View("Index", busModel);
        }
    }
}
