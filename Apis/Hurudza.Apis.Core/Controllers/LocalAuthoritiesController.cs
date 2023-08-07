using System.Net;
using System.Net.Mime;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
[Authorize]
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
    
    [HttpGet("{id:int}", Name = nameof(GetLocalAuthoritiesByProvinceId))]
    public async Task<IActionResult> GetLocalAuthoritiesByProvinceId(int id)
    {
        var localAuthorities = await (from l in _context.LocalAuthorities
            join d in _context.Districts on l.DistrictId equals d.Id
            join p in _context.Provinces on d.ProvinceId equals p.Id
            where p.Id == id
            select l).ToListAsync().ConfigureAwait(false);

        return Ok(localAuthorities);
    }
    
    [HttpGet("{id:int}", Name = nameof(GetLocalAuthoritiesByDistrictId))]
    public async Task<IActionResult> GetLocalAuthoritiesByDistrictId(int id)
    {
        var localAuthorities = await _context.LocalAuthorities.Where(d => d.DistrictId == id).ToListAsync().ConfigureAwait(false);

        return Ok(localAuthorities);
    }

    [HttpGet("{id:int}", Name = nameof(GetLocalAuthority))]
    public async Task<IActionResult> GetLocalAuthority(int id)
    {
        var localAuthority = await _context.LocalAuthorities.FirstOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);

        return localAuthority == null ? Ok(new ApiResponse((int)HttpStatusCode.NotFound)) : Ok(new ApiOkResponse(localAuthority));
    }
}