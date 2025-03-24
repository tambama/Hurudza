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
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class TillageServicesController : Controller
{
    private readonly HurudzaDbContext _context;
    private readonly IConfigurationProvider _configuration;

    public TillageServicesController(HurudzaDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet(Name = nameof(GetTillageServices))]
    public async Task<IActionResult> GetTillageServices()
    {
        var services = await _context.TillageServices
            .Where(ts => !ts.Deleted)
            .ProjectTo<TillageServiceViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(services);
    }
    
    [HttpGet("{programId}", Name = nameof(GetProgramTillageServices))]
    public async Task<IActionResult> GetProgramTillageServices(string programId)
    {
        var services = await _context.TillageServices
            .Where(ts => ts.TillageProgramId == programId && !ts.Deleted)
            .ProjectTo<TillageServiceViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(services);
    }
    
    [HttpGet("{farmId}", Name = nameof(GetReceivedTillageServices))]
    public async Task<IActionResult> GetReceivedTillageServices(string farmId)
    {
        var services = await _context.TillageServices
            .Where(ts => ts.RecipientFarmId == farmId && !ts.Deleted)
            .ProjectTo<TillageServiceViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(services);
    }

    [HttpGet("{id}", Name = nameof(GetTillageService))]
    public async Task<IActionResult> GetTillageService(string id)
    {
        var service = await _context.TillageServices
            .ProjectTo<TillageServiceViewModel>(_configuration)
            .FirstOrDefaultAsync(ts => ts.Id == id)
            .ConfigureAwait(false);

        return service == null 
            ? Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Tillage service does not exist")) 
            : Ok(new ApiOkResponse(service));
    }

    [HttpPost(Name = nameof(CreateTillageService))]
    public async Task<IActionResult> CreateTillageService([FromBody] TillageServiceViewModel model)
    {
        try
        {
            // Validate if program exists
            var program = await _context.TillagePrograms
                .FirstOrDefaultAsync(tp => tp.Id == model.TillageProgramId)
                .ConfigureAwait(false);
                
            if (program == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Tillage program not found"));
            }
            
            var tillageService = _configuration.CreateMapper().Map<TillageService>(model);
            
            // Add the service
            await _context.TillageServices.AddAsync(tillageService).ConfigureAwait(false);
            
            // Update the tilled hectares in the program if service is completed
            if (tillageService.IsCompleted)
            {
                program.TilledHectares += tillageService.Hectares;
                _context.Entry(program).State = EntityState.Modified;
            }
            
            await _context.SaveChangesAsync().ConfigureAwait(false);
            
            var newService = await _context.TillageServices
                .Where(ts => ts.Id == tillageService.Id)
                .ProjectTo<TillageServiceViewModel>(_configuration)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            
            return Ok(new ApiOkResponse(newService, "Tillage service successfully created"));
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error creating tillage service: {ex.Message}"));
        }
    }
    
    [HttpPut("{id}", Name = nameof(UpdateTillageService))]
    public async Task<IActionResult> UpdateTillageService(string id, [FromBody] TillageServiceViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        // Run this in a transaction to ensure data consistency
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var service = await _context.TillageServices
                .Include(ts => ts.TillageProgram)
                .FirstOrDefaultAsync(ts => ts.Id == id)
                .ConfigureAwait(false);
                
            if (service == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Tillage service not found"));
            }
            
            // Get the program
            var program = service.TillageProgram;
            if (program == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Tillage program not found for this service"));
            }
            
            // Track status change and hectare difference
            bool wasCompleted = service.IsCompleted;
            float oldHectares = service.Hectares;
            
            // Update service properties
            service.RecipientFarmId = model.RecipientFarmId;
            service.ServiceDate = model.ServiceDate;
            service.Hectares = model.Hectares;
            service.TillageType = model.TillageType;
            service.FieldId = model.FieldId;
            service.Notes = model.Notes;
            service.IsCompleted = model.IsCompleted;
            service.ServiceCost = model.ServiceCost;
            
            _context.Entry(service).State = EntityState.Modified;
            
            // Update program's tilled hectares based on service status changes
            if (!wasCompleted && service.IsCompleted)
            {
                // Service was marked as completed - add hectares
                program.TilledHectares += service.Hectares;
            }
            else if (wasCompleted && !service.IsCompleted)
            {
                // Service was marked as not completed - subtract old hectares
                program.TilledHectares -= oldHectares;
            }
            else if (wasCompleted && service.IsCompleted && oldHectares != service.Hectares)
            {
                // Service remained completed but hectares changed
                program.TilledHectares = program.TilledHectares - oldHectares + service.Hectares;
            }
            
            _context.Entry(program).State = EntityState.Modified;
            
            // Save changes and commit transaction
            await _context.SaveChangesAsync().ConfigureAwait(false);
            await transaction.CommitAsync();
            
            var updatedService = await _context.TillageServices
                .Where(ts => ts.Id == id)
                .ProjectTo<TillageServiceViewModel>(_configuration)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return Ok(new ApiOkResponse(updatedService, "Tillage service updated successfully"));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error updating tillage service: {ex.Message}"));
        }
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteTillageService))]
    public async Task<IActionResult> DeleteTillageService(string id)
    {
        // Run this in a transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var service = await _context.TillageServices
                .Include(ts => ts.TillageProgram)
                .FirstOrDefaultAsync(ts => ts.Id == id)
                .ConfigureAwait(false);

            if (service == null) return NotFound();

            // If service was completed, update the program's tilled hectares
            if (service.IsCompleted && service.TillageProgram != null)
            {
                var program = service.TillageProgram;
                program.TilledHectares -= service.Hectares;
                if (program.TilledHectares < 0) program.TilledHectares = 0;
                _context.Entry(program).State = EntityState.Modified;
            }

            // Mark service as deleted
            service.Deleted = true;
            service.IsActive = false;
            _context.Entry(service).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new ApiOkResponse(service, "Tillage service was deleted successfully"));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error deleting tillage service: {ex.Message}"));
        }
    }
    
    [HttpGet(Name = nameof(GetTillageStatistics))]
    public async Task<IActionResult> GetTillageStatistics()
    {
        // Get summary statistics for all tillage services
        var statistics = new
        {
            TotalPrograms = await _context.TillagePrograms.CountAsync(tp => tp.IsActive),
            TotalServices = await _context.TillageServices.CountAsync(ts => !ts.Deleted),
            CompletedServices = await _context.TillageServices.CountAsync(ts => !ts.Deleted && ts.IsCompleted),
            TotalPlannedHectares = await _context.TillagePrograms.Where(tp => tp.IsActive).SumAsync(tp => tp.TotalHectares),
            TotalTilledHectares = await _context.TillagePrograms.Where(tp => tp.IsActive).SumAsync(tp => tp.TilledHectares),
            TotalRevenue = await _context.TillageServices.Where(ts => !ts.Deleted).SumAsync(ts => ts.ServiceCost ?? 0)
        };

        return Ok(statistics);
    }
    
    [HttpGet("{year}", Name = nameof(GetTillageStatisticsByYear))]
    public async Task<IActionResult> GetTillageStatisticsByYear(int year)
    {
        // Get statistics for specific year
        var startDate = new DateTime(year, 1, 1);
        var endDate = new DateTime(year, 12, 31);
        
        var statistics = new
        {
            Year = year,
            TotalPrograms = await _context.TillagePrograms
                .CountAsync(tp => tp.IsActive && tp.StartDate >= startDate && tp.EndDate <= endDate),
                
            TotalServices = await _context.TillageServices
                .CountAsync(ts => !ts.Deleted && ts.ServiceDate >= startDate && ts.ServiceDate <= endDate),
                
            CompletedServices = await _context.TillageServices
                .CountAsync(ts => !ts.Deleted && ts.IsCompleted && ts.ServiceDate >= startDate && ts.ServiceDate <= endDate),
                
            TotalPlannedHectares = await _context.TillagePrograms
                .Where(tp => tp.IsActive && tp.StartDate >= startDate && tp.EndDate <= endDate)
                .SumAsync(tp => tp.TotalHectares),
                
            TotalTilledHectares = await _context.TillagePrograms
                .Where(tp => tp.IsActive && tp.StartDate >= startDate && tp.EndDate <= endDate)
                .SumAsync(tp => tp.TilledHectares),
                
            TotalRevenue = await _context.TillageServices
                .Where(ts => !ts.Deleted && ts.ServiceDate >= startDate && ts.ServiceDate <= endDate)
                .SumAsync(ts => ts.ServiceCost ?? 0)
        };

        return Ok(statistics);
    }
    
    [HttpGet("{id}/complete", Name = nameof(CompleteTillageService))]
    public async Task<IActionResult> CompleteTillageService(string id)
    {
        // Run this in a transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var service = await _context.TillageServices
                .Include(ts => ts.TillageProgram)
                .FirstOrDefaultAsync(ts => ts.Id == id)
                .ConfigureAwait(false);
                
            if (service == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Tillage service not found"));
            }
            
            // If already completed, return success
            if (service.IsCompleted)
            {
                return Ok(new ApiOkResponse(service, "Service already marked as completed"));
            }
            
            // Get the program
            var program = service.TillageProgram;
            if (program == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Tillage program not found for this service"));
            }
            
            // Mark as completed
            service.IsCompleted = true;
            _context.Entry(service).State = EntityState.Modified;
            
            // Update program's tilled hectares
            program.TilledHectares += service.Hectares;
            _context.Entry(program).State = EntityState.Modified;
            
            await _context.SaveChangesAsync().ConfigureAwait(false);
            await transaction.CommitAsync();
            
            var updatedService = await _context.TillageServices
                .Where(ts => ts.Id == id)
                .ProjectTo<TillageServiceViewModel>(_configuration)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return Ok(new ApiOkResponse(updatedService, "Tillage service marked as completed"));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error completing tillage service: {ex.Message}"));
        }
    }
    
    [HttpGet("{programId}/summary", Name = nameof(GetProgramServicesSummary))]
    public async Task<IActionResult> GetProgramServicesSummary(string programId)
    {
        try
        {
            // Check if program exists
            var program = await _context.TillagePrograms
                .FirstOrDefaultAsync(tp => tp.Id == programId)
                .ConfigureAwait(false);
                
            if (program == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Tillage program not found"));
            }
            
            // Get summary of services by type
            var servicesByType = await _context.TillageServices
                .Where(ts => ts.TillageProgramId == programId && !ts.Deleted)
                .GroupBy(ts => ts.TillageType)
                .Select(g => new
                {
                    TillageType = g.Key,
                    TillageTypeName = g.Key.ToString(),
                    TotalServices = g.Count(),
                    CompletedServices = g.Count(ts => ts.IsCompleted),
                    TotalHectares = g.Sum(ts => ts.Hectares),
                    TotalRevenue = g.Sum(ts => ts.ServiceCost ?? 0)
                })
                .ToListAsync()
                .ConfigureAwait(false);
                
            // Get summary of services by farm
            var servicesByFarm = await _context.TillageServices
                .Where(ts => ts.TillageProgramId == programId && !ts.Deleted)
                .GroupBy(ts => ts.RecipientFarmId)
                .Select(g => new
                {
                    FarmId = g.Key,
                    FarmName = g.First().RecipientFarm.Name,
                    TotalServices = g.Count(),
                    CompletedServices = g.Count(ts => ts.IsCompleted),
                    TotalHectares = g.Sum(ts => ts.Hectares),
                    TotalRevenue = g.Sum(ts => ts.ServiceCost ?? 0)
                })
                .ToListAsync()
                .ConfigureAwait(false);
                
            // Create aggregate summary
            var summary = new
            {
                ProgramId = programId,
                ProgramName = program.Name,
                TotalServices = await _context.TillageServices
                    .CountAsync(ts => ts.TillageProgramId == programId && !ts.Deleted),
                CompletedServices = await _context.TillageServices
                    .CountAsync(ts => ts.TillageProgramId == programId && !ts.Deleted && ts.IsCompleted),
                TotalHectares = await _context.TillageServices
                    .Where(ts => ts.TillageProgramId == programId && !ts.Deleted)
                    .SumAsync(ts => ts.Hectares),
                TotalRevenue = await _context.TillageServices
                    .Where(ts => ts.TillageProgramId == programId && !ts.Deleted)
                    .SumAsync(ts => ts.ServiceCost ?? 0),
                ServicesByType = servicesByType,
                ServicesByFarm = servicesByFarm
            };
            
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error getting program services summary: {ex.Message}"));
        }
    }
    
    [HttpGet("{farmId}/revenue", Name = nameof(GetFarmTillageRevenue))]
    public async Task<IActionResult> GetFarmTillageRevenue(string farmId)
    {
        try
        {
            // Check if farm exists
            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.Id == farmId)
                .ConfigureAwait(false);
                
            if (farm == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Farm not found"));
            }
            
            // Get revenue by month
            var currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, 1, 1);
            var endDate = new DateTime(currentYear, 12, 31);
            
            var revenueByMonth = await _context.TillageServices
                .Where(ts => ts.RecipientFarmId == farmId && !ts.Deleted && 
                       ts.ServiceDate >= startDate && ts.ServiceDate <= endDate)
                .GroupBy(ts => ts.ServiceDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(ts => ts.ServiceCost ?? 0)
                })
                .ToListAsync()
                .ConfigureAwait(false);
                
            // Fill in missing months
            var completeMonthlyRevenue = Enumerable.Range(1, 12)
                .Select(month => new
                {
                    Month = month,
                    MonthName = new DateTime(currentYear, month, 1).ToString("MMMM"),
                    Revenue = revenueByMonth.FirstOrDefault(r => r.Month == month)?.Revenue ?? 0
                })
                .ToList();
                
            return Ok(completeMonthlyRevenue);
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error getting farm tillage revenue: {ex.Message}"));
        }
    }
}