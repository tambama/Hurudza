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
public class ProvincesController : Controller
{
    private readonly HurudzaDbContext _context;

    public ProvincesController(HurudzaDbContext context)
    {
        _context = context;
    }
    
    [HttpGet(Name = nameof(GetProvinces))]
    public async Task<IActionResult> GetProvinces()
    {
        var provinces = await _context.Provinces.OrderBy(p => p.Name).ToListAsync().ConfigureAwait(false);

        return Ok(provinces);
    }

    [HttpGet("{id}", Name = nameof(GetProvince))]
    public async Task<IActionResult> GetProvince(string id)
    {
        var province = await _context.Provinces.FirstOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);

        return province == null ? Ok(new ApiResponse((int)HttpStatusCode.NotFound)) : Ok(new ApiOkResponse(province));
    }
}