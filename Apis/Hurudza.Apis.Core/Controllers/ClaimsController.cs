using System.Net.Mime;
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
public class ClaimsController : Controller
{
    private readonly HurudzaDbContext _context;

    public ClaimsController(HurudzaDbContext context)
    {
        _context = context;
    }
    
    // GET
    [HttpGet(Name = nameof(GetAllClaims))]
    public async Task<IActionResult> GetAllClaims()
    {
        var claims = await _context.Claims.ToListAsync().ConfigureAwait(false);

        return Ok(claims);
    }
}