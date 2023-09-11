using System.Net;
using System.Net.Mime;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FieldLocationsController : Controller
{
    private readonly HurudzaDbContext _context;

    public FieldLocationsController(HurudzaDbContext context)
    {
        _context = context;
    }
    
    [HttpGet(Name = nameof(GetFieldLocations))]
    public async Task<IActionResult> GetFieldLocations()
    {
        var locations = await _context.Locations.OfType<FieldLocation>().ToListAsync().ConfigureAwait(false);

        return Ok(locations);
    }
    
    [HttpGet("{id}", Name = nameof(GetFieldLocationsByFieldId))]
    public async Task<IActionResult> GetFieldLocationsByFieldId(string id)
    {
        var locations = await _context.Locations.OfType<FieldLocation>().Where(l => l.FieldId == id).ToListAsync().ConfigureAwait(false);

        return Ok(locations);
    }

    [HttpGet("{id}", Name = nameof(GetFieldLocation))]
    public async Task<IActionResult> GetFieldLocation(string id)
    {
        var location = await _context.Locations.OfType<FieldLocation>().FirstOrDefaultAsync(l => l.Id == id)
            .ConfigureAwait(false);

        if (location == null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    public async Task<IActionResult> CreateFieldLocation([FromBody] FieldLocation model)
    {
        await _context.AddAsync(model).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(model, "Location added successfully"));
    }
    
    [HttpPut("{id}", Name = nameof(UpdateFieldLocation))]
    public async Task<IActionResult> UpdateFieldLocation(string id, [FromBody] FieldLocation model)
    {
        var location = await _context.Locations.OfType<FieldLocation>().FirstOrDefaultAsync(l => l.Id == model.Id)
            .ConfigureAwait(false);
        
        if (location == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Location does not exist"));
        }
        
        _context.Update(location);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(model, "Location updated successfully"));
    }

    [HttpDelete("{id}", Name = nameof(DeleteFieldLocation))]
    public async Task<IActionResult> DeleteFieldLocation(string id)
    {
        var location = await _context.Locations.OfType<FieldLocation>().FirstOrDefaultAsync(l => l.Id == id)
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