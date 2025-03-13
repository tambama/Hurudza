using System.Net;
using System.Net.Mime;
using Asp.Versioning;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class CropsController : Controller
{
    private readonly HurudzaDbContext _context;

    public CropsController(HurudzaDbContext context)
    {
        _context = context;
    }
    
    [HttpGet(Name = nameof(GetCrops))]
    public async Task<IActionResult> GetCrops()
    {
        var crops = await _context.Crops.ToListAsync().ConfigureAwait(false);

        return Ok(crops);
    }

    [HttpGet("{id}", Name = nameof(GetCrop))]
    public async Task<IActionResult> GetCrop(string id)
    {
        var crop = await _context.Crops.SingleOrDefaultAsync(c => c.Id == id).ConfigureAwait(false);

        return crop == null ? Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Crop does not exist")) : Ok(crop);
    }
}