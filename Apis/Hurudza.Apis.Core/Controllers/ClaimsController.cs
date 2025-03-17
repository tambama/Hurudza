using System.Net;
using System.Net.Mime;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

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
    private readonly IConfigurationProvider _configuration;

    public ClaimsController(HurudzaDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    [HttpGet(Name = nameof(GetAllClaims))]
    public async Task<IActionResult> GetAllClaims()
    {
        var claims = await _context.Claims
            .ProjectTo<ClaimViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(claims);
    }
    
    [HttpGet(Name = nameof(GetClaimsByType))]
    public async Task<IActionResult> GetClaimsByType([FromQuery] string claimType)
    {
        var claims = await _context.Claims
            .Where(c => c.ClaimType == claimType)
            .ProjectTo<ClaimViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(claims);
    }
    
    [HttpGet("{id}", Name = nameof(GetClaim))]
    public async Task<IActionResult> GetClaim(int id)
    {
        var claim = await _context.Claims
            .ProjectTo<ClaimViewModel>(_configuration)
            .FirstOrDefaultAsync(c => c.Id == id)
            .ConfigureAwait(false);

        if (claim == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Claim not found"));

        return Ok(new ApiOkResponse(claim));
    }
    
    [HttpPost(Name = nameof(CreateClaim))]
    [Authorize(Roles = "SystemAdministrator")]
    public async Task<IActionResult> CreateClaim([FromBody] ClaimViewModel model)
    {
        // Check if claim already exists
        var existingClaim = await _context.Claims
            .FirstOrDefaultAsync(c => c.ClaimType == model.ClaimType && c.ClaimValue == model.ClaimValue)
            .ConfigureAwait(false);

        if (existingClaim != null)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Claim already exists"));

        var claim = new IdentityClaim
        {
            ClaimType = model.ClaimType,
            ClaimValue = model.ClaimValue
        };

        await _context.Claims.AddAsync(claim);
        await _context.SaveChangesAsync();

        var createdClaim = await _context.Claims
            .ProjectTo<ClaimViewModel>(_configuration)
            .FirstOrDefaultAsync(c => c.Id == claim.Id)
            .ConfigureAwait(false);

        return Ok(new ApiOkResponse(createdClaim, "Claim created successfully"));
    }
    
    [HttpPut("{id}", Name = nameof(UpdateClaim))]
    [Authorize(Roles = "SystemAdministrator")]
    public async Task<IActionResult> UpdateClaim(int id, [FromBody] ClaimViewModel model)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Claim not found"));

        // Check if updated claim would conflict with existing claim
        var existingClaim = await _context.Claims
            .FirstOrDefaultAsync(c => c.ClaimType == model.ClaimType && c.ClaimValue == model.ClaimValue && c.Id != id)
            .ConfigureAwait(false);

        if (existingClaim != null)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Claim with these values already exists"));

        claim.ClaimType = model.ClaimType;
        claim.ClaimValue = model.ClaimValue;

        _context.Claims.Update(claim);
        await _context.SaveChangesAsync();

        var updatedClaim = await _context.Claims
            .ProjectTo<ClaimViewModel>(_configuration)
            .FirstOrDefaultAsync(c => c.Id == claim.Id)
            .ConfigureAwait(false);

        return Ok(new ApiOkResponse(updatedClaim, "Claim updated successfully"));
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteClaim))]
    [Authorize(Roles = "SystemAdministrator")]
    public async Task<IActionResult> DeleteClaim(int id)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Claim not found"));

        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync();

        return Ok(new ApiOkResponse(null, "Claim deleted successfully"));
    }
    
    [HttpGet(Name = nameof(GetClaimTypes))]
    public async Task<IActionResult> GetClaimTypes()
    {
        var claimTypes = await _context.Claims
            .Select(c => c.ClaimType)
            .Distinct()
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(claimTypes);
    }
}