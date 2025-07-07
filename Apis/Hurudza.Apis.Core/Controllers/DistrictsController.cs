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
public class DistrictsController : Controller
{
    private readonly HurudzaDbContext _context;

    public DistrictsController(HurudzaDbContext context)
    {
        _context = context;
    }
    
    [HttpGet(Name = nameof(GetDistricts))]
    public async Task<IActionResult> GetDistricts()
    {
        var districts = await _context.Districts.ToListAsync().ConfigureAwait(false);

        return Ok(districts);
    }
    
    [HttpGet("{id}", Name = nameof(GetDistrictsByProvinceId))]
    public async Task<IActionResult> GetDistrictsByProvinceId(string id)
    {
        var districts = await _context.Districts.Where(d => d.ProvinceId == id).ToListAsync().ConfigureAwait(false);

        return Ok(districts);
    }

    [HttpGet("{id:int}", Name = nameof(GetDistrict))]
    public async Task<IActionResult> GetDistrict(string id)
    {
        var district = await _context.Districts.FirstOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);

        return district == null ? Ok(new ApiResponse((int)HttpStatusCode.NotFound)) : Ok(new ApiOkResponse(district));
    }
}