using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;
using TiVerse.Application.DTO;

namespace TiVerse.WebUI.Controllers
{
    public class TrainController : Controller
    {
        private readonly TiVerseDbContext _context;
        private readonly IMapper _mapper;

        public TrainController(TiVerseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<Trip> trainRoutes = new List<Trip>();

            trainRoutes = await _context.Trips
                .Where(t => t.Transport == "Train")
                .Take(50)
                .ToListAsync();

            var trainRoutesToDTO = _mapper.Map<List<RouteDTO>>(trainRoutes);

            return View("Index", trainRoutesToDTO);
        }
    }
}
