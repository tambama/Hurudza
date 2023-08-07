using System.Net;
using System.Net.Mime;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FarmLocationsController : Controller
{
    private readonly HurudzaDbContext _context;

    public FarmLocationsController(HurudzaDbContext context)
    {
        _context = context;
    }
    
    [HttpGet(Name = nameof(GetFarmLocations))]
    public async Task<IActionResult> GetFarmLocations()
    {
        var locations = await _context.Locations.OfType<FarmLocation>().ToListAsync().ConfigureAwait(false);

        return Ok(locations);
    }
    
    [HttpGet("{id}", Name = nameof(GetFarmLocationsByFarmId))]
    public async Task<IActionResult> GetFarmLocationsByFarmId(string id)
    {
        var locations = await _context.Locations.OfType<FarmLocation>().Where(l => l.FarmId == id).ToListAsync().ConfigureAwait(false);

        return Ok(locations);
    }

    [HttpGet("{id}", Name = nameof(GetFarmLocation))]
    public async Task<IActionResult> GetFarmLocation(string id)
    {
        var location = await _context.Locations.OfType<FarmLocation>().FirstOrDefaultAsync(l => l.Id == id)
            .ConfigureAwait(false);

        if (location == null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    public async Task<IActionResult> CreateFarmLocation([FromBody] FarmLocation model)
    {
        await _context.AddAsync(model).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(model, "Location added successfully"));
    }
    
    [HttpPut("{id}", Name = nameof(UpdateFarmLocation))]
    public async Task<IActionResult> UpdateFarmLocation(string id, [FromBody] FarmLocation model)
    {
        var location = await _context.Locations.OfType<FarmLocation>().FirstOrDefaultAsync(l => l.Id == model.Id)
            .ConfigureAwait(false);
        
        if (location == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Location does not exist"));
        }
        
        _context.Update(location);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(model, "Location updated successfully"));
    }

    [HttpDelete("{id}", Name = nameof(DeleteFarmLocation))]
    public async Task<IActionResult> DeleteFarmLocation(string id)
    {
        var location = await _context.Locations.OfType<FarmLocation>().FirstOrDefaultAsync(l => l.Id == id)
            .ConfigureAwait(false);
        
        if (location == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Location does not exist"));
        }
        
        _context.Remove(location);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(location, "Location deleted successfully"));
    }
}