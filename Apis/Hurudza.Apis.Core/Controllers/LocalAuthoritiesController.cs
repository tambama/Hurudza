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
public class LocalAuthoritiesController : Controller
{
    private readonly HurudzaDbContext _context;

    public LocalAuthoritiesController(HurudzaDbContext context)
    {
        _context = context;
    }
    
    [HttpGet(Name = nameof(GetLocalAuthorities))]
    public async Task<IActionResult> GetLocalAuthorities()
    {
        var localAuthorities = await _context.LocalAuthorities.ToListAsync().ConfigureAwait(false);

        return Ok(localAuthorities);
    }
    
    [HttpGet("{id}", Name = nameof(GetLocalAuthoritiesByProvinceId))]
    public async Task<IActionResult> GetLocalAuthoritiesByProvinceId(string id)
    {
        var localAuthorities = await (from l in _context.LocalAuthorities
            join d in _context.Districts on l.DistrictId equals d.Id
            join p in _context.Provinces on d.ProvinceId equals p.Id
            where p.Id == id
            select l).ToListAsync().ConfigureAwait(false);

        return Ok(localAuthorities);
    }
    
    [HttpGet("{id}", Name = nameof(GetLocalAuthoritiesByDistrictId))]
    public async Task<IActionResult> GetLocalAuthoritiesByDistrictId(string id)
    {
        var localAuthorities = await _context.LocalAuthorities.Where(d => d.DistrictId == id).ToListAsync().ConfigureAwait(false);

        return Ok(localAuthorities);
    }

    [HttpGet("{id}", Name = nameof(GetLocalAuthority))]
    public async Task<IActionResult> GetLocalAuthority(string id)
    {
        var localAuthority = await _context.LocalAuthorities.FirstOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);

        return localAuthority == null ? Ok(new ApiResponse((int)HttpStatusCode.NotFound)) : Ok(new ApiOkResponse(localAuthority));
    }
}