using System.Net;
using System.Net.Mime;
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
public class WardsController : Controller
{
    private readonly HurudzaDbContext _context;

    public WardsController(HurudzaDbContext context)
    {
        _context = context;
    }
    
    [HttpGet(Name = nameof(GetWards))]
    public async Task<IActionResult> GetWards()
    {
        var wards = await _context.Wards.ToListAsync().ConfigureAwait(false);

        return Ok(wards);
    }
    
    [HttpGet("{id}", Name = nameof(GetWardsByProvinceId))]
    public async Task<IActionResult> GetWardsByProvinceId(string id)
    {
        var wards = await _context.Wards.Where(d => d.ProvinceId == id).ToListAsync().ConfigureAwait(false);

        return Ok(wards);
    }
    
    [HttpGet("{id}", Name = nameof(GetWardsByDistrictId))]
    public async Task<IActionResult> GetWardsByDistrictId(string id)
    {
        var wards = await _context.Wards.Where(d => d.DistrictId == id).ToListAsync().ConfigureAwait(false);

        return Ok(wards);
    }
    
    [HttpGet("{id}", Name = nameof(GetWardsByLocalAuthorityId))]
    public async Task<IActionResult> GetWardsByLocalAuthorityId(string id)
    {
        var wards = await _context.Wards.Where(d => d.LocalAuthorityId == id).ToListAsync().ConfigureAwait(false);

        return Ok(wards);
    }

    [HttpGet("{id}", Name = nameof(GetWard))]
    public async Task<IActionResult> GetWard(string id)
    {
        var ward = await _context.Wards.FirstOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);

        return ward == null ? Ok(new ApiResponse((int)HttpStatusCode.NotFound)) : Ok(new ApiOkResponse(ward));
    }
}