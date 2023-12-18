using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;

namespace TiVerse.WebUI.Controllers
{
    public class TrainController : Controller
    {
        private readonly TiVerseDbContext _context;

        public TrainController(TiVerseDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Trip> trainModel = new List<Trip>();
            trainModel = await _context.Trips.Where(t => t.Transport == "Train").Take(50).ToListAsync();
            return View("Index", trainModel);
        }
    }
}
