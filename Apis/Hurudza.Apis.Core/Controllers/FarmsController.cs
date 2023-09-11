using System.Net;
using System.Net.Mime;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FarmsController : Controller
{
    private readonly HurudzaDbContext _context;
    private readonly IConfigurationProvider _configuration;

    public FarmsController(HurudzaDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet(Name = nameof(GetFarms))]
    public async Task<IActionResult> GetFarms()
    {
        var farms = await _context.Farms
            .ProjectTo<FarmViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(farms);
    }

    [HttpPost(Name = nameof(CreateFarm))]
    public async Task<IActionResult> CreateFarm([FromBody] FarmViewModel model)
    {
        var farm = _configuration.CreateMapper().Map<Farm>(model);

        await _context.AddAsync(farm).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(farm, "Farm successfully created"));
    }
    
    [HttpPost("{id}", Name = nameof(UpdateFarm))]
    public async Task<IActionResult> UpdateFarm(string id, [FromBody] FarmViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == id).ConfigureAwait(false);
        if (farm == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Resource not found"));
        }

        farm = _configuration.CreateMapper().Map<Farm>(model);

        _context.Entry(farm).State = EntityState.Modified;
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(farm, $"{farm.Name} successfully updated"));
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteFarm))]
    public async Task<IActionResult> DeleteFarm(string id)
    {
        var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == id).ConfigureAwait(false);

        if (farm == null) return NotFound();

        farm.Deleted = true;
        farm.IsActive = false;

        _context.Entry(farm).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return Ok(new ApiOkResponse(farm, $"{farm.Name} was deleted successfully"));
    }
}