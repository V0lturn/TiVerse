using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TiVerse.Application.DTO;
using TiVerse.Application.UseCase;
using TiVerse.Infrastructure.AppDbContext;
namespace TiVerse.WebUI.Controllers
{
    public class MainPageController : Controller
    {
        private readonly TiVerseDbContext _context;
        private readonly IMapper _mapper;

        public MainPageController(TiVerseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> FindRoutes(string Departure, string Destination, DateTime Date, string Transport)
        {
            var findRoutes = new FindRouteUseCase(_context);
            var routes = await findRoutes.Routes(Departure, Destination, Date, Transport);

            if (routes != null)
            {
                return View("FindedRoutes", routes);
            }
            else
            {
                return RedirectToAction("Index", "MainPage");
            }
        }
    }
}
