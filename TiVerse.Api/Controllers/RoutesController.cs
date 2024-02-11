using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TiVerse.Application.Interfaces.ITransportRepositoryInterface;
using Microsoft.EntityFrameworkCore;
using TiVerse.Application.Pagination;
using TiVerse.Core.Entity;
using TiVerse.Application.ViewModels;
namespace TiVerse.Api.Controllers;

[Route("routes")]
[Authorize]
[ApiController]
public class RoutesController : ControllerBase
{
    private readonly ITransportRepository _transportRepository;
    private const int defaultPageSize = 50;

    public RoutesController(ITransportRepository transportRepository)
    {
        _transportRepository = transportRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var allRoutes = await _transportRepository.GetAllRoutes().ToListAsync();

        return Ok(allRoutes);
    }

    // GET: routes/page/1
    [HttpGet("page/{pageNumber}")]
    public async Task<IActionResult> GetRoutes(int pageNumber = 1)
    {
        try
        {
            var tripQuery = _transportRepository.GetAllRoutes();

            var pagedList = await PagedList<Trip>.CreateAsync(tripQuery, pageNumber, defaultPageSize);

            if (pagedList == null)
                return NotFound();

            return Ok(pagedList);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    // GET: routes/transport/bus
    [HttpGet("transport/{transport}")]
    public async Task<IActionResult> GetRoutes(string transport = "bus")
    {
        try
        {
            var routesByTransport = await _transportRepository.GetRoutesByTransport(transport).ToListAsync();

            if (routesByTransport == null)
                return NotFound();

            return Ok(routesByTransport);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    // GET: routes/transport/bus/page/1
    [HttpGet("transport/{transport}/page/{pageNumber}")]
    public async Task<IActionResult> GetRoutes(string transport = "bus", int pageNumber = 1)
    {
        try
        {
            var routesByTransport = _transportRepository.GetRoutesByTransport(transport);

            var pagedList = await PagedList<Trip>.CreateAsync(routesByTransport, pageNumber, defaultPageSize);

            if (pagedList == null || pagedList.TotalCount <= 0)
                return NotFound("No routes found for the specified transport.");

            return Ok(pagedList);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    public ActionResult<Trip> CreateRoute([FromBody] RouteViewModel viewModel)
    {
        if (viewModel == null)
        {
            return BadRequest("Invalid JSON data. Route not created.");
        }

        if (_transportRepository.CreateRoute(viewModel))
        {
            return Ok("Route created successfully");
        }
        else
        {
            return BadRequest("Invalid route parameters. Route not created.");
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTask(Guid id, RouteViewModel updatedRoute)
    {
        try
        {
            bool updateSuccessful = await _transportRepository.UpdateRoute(id, updatedRoute);

            if (updateSuccessful)
            {
                return Ok("Route updated successfully");
            }
            else
            {
                return NotFound($"Route with ID {id} not found or another error happend");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoute(Guid id)
    {
        bool removeSuccessfully = await _transportRepository.DeleteRoute(id);

        if (removeSuccessfully)
        {
            return Ok("Route deleted successfully");
        }
        else
        {
            return NotFound($"Route with ID {id} not found or another error happend");
        }
    }

}