using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;
using TiVerse.Application.DTO;

namespace TiVerse.WebUI.Controllers
{
    public class PlaneController : Controller
    {
        private readonly TiVerseDbContext _context;
        private readonly IMapper _mapper;

        public PlaneController(TiVerseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<Trip> planeRoutes = new List<Trip>();

            planeRoutes = await _context.Trips
                .Where(t => t.Transport == "Plane")
                .Take(50)
                .ToListAsync();

            var planeRoutesToDto = _mapper.Map<List<RouteDTO>>(planeRoutes);
            return View(planeRoutesToDto);
        }
    }
}
