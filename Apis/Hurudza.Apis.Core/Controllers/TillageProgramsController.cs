using System.Net;
using System.Net.Mime;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Tillage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class TillageProgramsController : Controller
{
    private readonly HurudzaDbContext _context;
    private readonly IConfigurationProvider _configuration;

    public TillageProgramsController(HurudzaDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet(Name = nameof(GetTillagePrograms))]
    public async Task<IActionResult> GetTillagePrograms()
    {
        var programs = await _context.TillagePrograms
            .Where(tp => tp.IsActive)
            .ProjectTo<TillageProgramViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(programs);
    }
    
    [HttpGet("{farmId}", Name = nameof(GetFarmTillagePrograms))]
    public async Task<IActionResult> GetFarmTillagePrograms(string farmId)
    {
        var programs = await _context.TillagePrograms
            .Where(tp => tp.FarmId == farmId && tp.IsActive)
            .ProjectTo<TillageProgramViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(programs);
    }

    [HttpGet("{id}", Name = nameof(GetTillageProgram))]
    public async Task<IActionResult> GetTillageProgram(string id)
    {
        var program = await _context.TillagePrograms
            .ProjectTo<TillageProgramViewModel>(_configuration)
            .FirstOrDefaultAsync(tp => tp.Id == id)
            .ConfigureAwait(false);

        return program == null 
            ? Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Tillage program does not exist")) 
            : Ok(new ApiOkResponse(program));
    }

    [HttpPost(Name = nameof(CreateTillageProgram))]
    public async Task<IActionResult> CreateTillageProgram([FromBody] TillageProgramViewModel model)
    {
        try
        {
            var tillageProgram = _configuration.CreateMapper().Map<TillageProgram>(model);
            
            await _context.TillagePrograms.AddAsync(tillageProgram).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            
            var newProgram = await _context.TillagePrograms
                .Where(tp => tp.Id == tillageProgram.Id)
                .ProjectTo<TillageProgramViewModel>(_configuration)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            
            return Ok(new ApiOkResponse(newProgram, "Tillage program successfully created"));
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error creating tillage program: {ex.Message}"));
        }
    }
    
    [HttpPut("{id}", Name = nameof(UpdateTillageProgram))]
    public async Task<IActionResult> UpdateTillageProgram(string id, [FromBody] TillageProgramViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var program = await _context.TillagePrograms
            .FirstOrDefaultAsync(tp => tp.Id == id)
            .ConfigureAwait(false);
            
        if (program == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Tillage program not found"));
        }

        // Update properties
        program.Name = model.Name;
        program.Description = model.Description;
        program.StartDate = model.StartDate;
        program.EndDate = model.EndDate;
        program.TotalHectares = model.TotalHectares;
        program.IsActive = model.IsActive;

        _context.Entry(program).State = EntityState.Modified;
        await _context.SaveChangesAsync().ConfigureAwait(false);

        var updatedProgram = await _context.TillagePrograms
            .Where(tp => tp.Id == id)
            .ProjectTo<TillageProgramViewModel>(_configuration)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return Ok(new ApiOkResponse(updatedProgram, "Tillage program successfully updated"));
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteTillageProgram))]
    public async Task<IActionResult> DeleteTillageProgram(string id)
    {
        // Use a transaction to ensure data consistency when deleting program and services
        using var transaction = await _context.Database.BeginTransactionAsync();
    
        try
        {
            var program = await _context.TillagePrograms
                .FirstOrDefaultAsync(tp => tp.Id == id)
                .ConfigureAwait(false);

            if (program == null) return NotFound();

            // First, find and delete all associated tillage services
            var services = await _context.TillageServices
                .Where(ts => ts.TillageProgramId == id)
                .ToListAsync();
            
            foreach (var service in services)
            {
                service.Deleted = true;
                service.IsActive = false;
                _context.Entry(service).State = EntityState.Modified;
            }

            // Then mark the program as deleted
            program.Deleted = true;
            program.IsActive = false;
            _context.Entry(program).State = EntityState.Modified;

            // Save all changes and commit the transaction
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new ApiOkResponse(program, $"Tillage program '{program.Name}' and all associated services were deleted successfully"));
        }
        catch (Exception ex)
        {
            // Roll back transaction if anything goes wrong
            await transaction.RollbackAsync();
            Log.Error(ex, "Error in DeleteTillageProgram");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error deleting tillage program: {ex.Message}"));
        }
    }
    
    [HttpGet(Name = nameof(GetTillageSummary))]
    public async Task<IActionResult> GetTillageSummary()
    {
        var summary = await _context.TillagePrograms
            .Where(tp => tp.IsActive)
            .GroupBy(tp => tp.FarmId)
            .Select(g => new TillageSummaryViewModel
            {
                FarmId = g.Key,
                FarmName = g.First().Farm.Name,
                TotalPlanned = g.Sum(tp => tp.TotalHectares),
                TotalTilled = g.Sum(tp => tp.TilledHectares),
                TotalServices = g.Sum(tp => tp.TillageServices.Count),
                CompletedServices = g.Sum(tp => tp.TillageServices.Count(ts => ts.IsCompleted)),
                TotalRevenue = g.Sum(tp => tp.TillageServices.Sum(ts => ts.ServiceCost ?? 0))
            })
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(summary);
    }
    
    [HttpGet("{farmId}", Name = nameof(GetFarmTillageSummary))]
    public async Task<IActionResult> GetFarmTillageSummary(string farmId)
    {
        var farm = await _context.Farms
            .FirstOrDefaultAsync(f => f.Id == farmId)
            .ConfigureAwait(false);
            
        if (farm == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Farm not found"));
        }
        
        var summary = await _context.TillagePrograms
            .Where(tp => tp.FarmId == farmId && tp.IsActive)
            .GroupBy(tp => tp.FarmId)
            .Select(g => new TillageSummaryViewModel
            {
                FarmId = g.Key,
                FarmName = g.First().Farm.Name,
                TotalPlanned = g.Sum(tp => tp.TotalHectares),
                TotalTilled = g.Sum(tp => tp.TilledHectares),
                TotalServices = g.Sum(tp => tp.TillageServices.Count),
                CompletedServices = g.Sum(tp => tp.TillageServices.Count(ts => ts.IsCompleted)),
                TotalRevenue = g.Sum(tp => tp.TillageServices.Sum(ts => ts.ServiceCost ?? 0))
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (summary == null)
        {
            // Return empty summary with farm details
            summary = new TillageSummaryViewModel
            {
                FarmId = farmId,
                FarmName = farm.Name,
                TotalPlanned = 0,
                TotalTilled = 0,
                TotalServices = 0,
                CompletedServices = 0,
                TotalRevenue = 0
            };
        }

        return Ok(summary);
    }
}