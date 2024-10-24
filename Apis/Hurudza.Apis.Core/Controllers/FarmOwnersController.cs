using System.Net;
using System.Net.Mime;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Exception = System.Exception;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FarmOwnersController : Controller
{
    private readonly HurudzaDbContext _context;
    private readonly IConfigurationProvider _configuration;

    public FarmOwnersController(HurudzaDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    [HttpGet(Name = nameof(GetFarmOwners))]
    public async Task<IActionResult> GetFarmOwners()
    {
        var farmOwners = await _context.FarmOwners.Where(e => e.IsActive).ProjectTo<FarmOwnerViewModel>(_configuration).ToListAsync().ConfigureAwait(false);

        return Ok(farmOwners);
    }

    [HttpGet("{id}", Name = nameof(GetFarmOwner))]
    public async Task<IActionResult> GetFarmOwner(string id)
    {
        var farmOwner = await _context.FarmOwners.SingleOrDefaultAsync(c => c.Id == id).ConfigureAwait(false);

        return farmOwner == null
            ? Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Farm Owner does not exist"))
            : Ok(new ApiOkResponse(farmOwner));
    }

    [HttpPost(Name = nameof(CreateFarmOwner))]
    public async Task<IActionResult> CreateFarmOwner([FromBody] FarmOwnerViewModel model)
    {
        var farmOwner = _configuration.CreateMapper().Map<FarmOwner>(model);
        
        await _context.FarmOwners.AddAsync(farmOwner).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
        var newOwnership = await _context.FarmOwners.ProjectTo<FarmOwnerViewModel>(_configuration).SingleOrDefaultAsync(c => c.Id == farmOwner.Id).ConfigureAwait(false);
        
        return Ok(new ApiOkResponse(farmOwner, "Created Farm Owner successfully"));
    }

    [HttpPut("{id}", Name = nameof(UpdateFarmOwner))]
    public async Task<IActionResult> UpdateFarmOwner(string id, [FromBody] FarmOwnerViewModel model)
    {
        try
        {
            if (id != model.Id)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "FarmOwner not found"));
            }

            var farmOwner = await _context.FarmOwners.FirstOrDefaultAsync(f => f.Id == id).ConfigureAwait(false);
            if (farmOwner == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "FarmOwner not found"));
            }

            farmOwner.FarmId = model.FarmId;
            farmOwner.EntityId = model.EntityId;
            farmOwner.OwnershipType = model.OwnershipType;
            farmOwner.StartOfOwnership = model.StartOfOwnership;
            farmOwner.EndOfOwnership = model.EndOfOwnership;

            _context.Update(farmOwner);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Ok(new ApiOkResponse(farmOwner, "Successfully updated FarmOwner"));
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, "Failed to update FarmOwner"));
        }
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteFarmOwner))]
    public async Task<IActionResult> DeleteFarmOwner(string id)
    {
        var farmOwner = await _context.FarmOwners.FirstOrDefaultAsync(f => f.Id == id).ConfigureAwait(false);

        if (farmOwner == null) return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "FarmOwner not found"));

        farmOwner.Deleted = true;
        farmOwner.IsActive = false;

        _context.Update(farmOwner);
        await _context.SaveChangesAsync();

        return Ok(new ApiOkResponse(farmOwner, $"Owner {farmOwner.Entity.Name} was deleted successfully for {farmOwner.Farm.Name}"));
    }
}