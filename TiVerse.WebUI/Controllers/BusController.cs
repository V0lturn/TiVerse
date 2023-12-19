using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;
using TiVerse.Application.DTO;

namespace TiVerse.WebUI.Controllers
{
    public class BusController : Controller
    {
        private readonly TiVerseDbContext _context;
        private readonly IMapper _mapper;

        public BusController(TiVerseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index() 
        {
            List<Trip> busRoutes = new List<Trip>();

            busRoutes = await _context.Trips
                .Where(t => t.Transport == "Bus")
                .Take(50)
                .ToListAsync();

            var busRoutesToDTO = _mapper.Map<List<RouteDTO>>(busRoutes);
            return View("Index", busRoutesToDTO);
        }
    }
}
