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
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FieldCropsController : Controller
{
    private readonly HurudzaDbContext _context;
    private readonly IConfigurationProvider _configuration;

    public FieldCropsController(HurudzaDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    [HttpGet(Name = nameof(GetFieldCrops))]
    public async Task<IActionResult> GetFieldCrops()
    {
        var fieldCrops = await _context.FieldCrops
            .ProjectTo<FieldCropViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(fieldCrops);
    }

    [HttpGet("{id}", Name = nameof(GetFieldFieldCrops))]
    public async Task<IActionResult> GetFieldFieldCrops(string id)
    {
        var fieldCrops = await _context.FieldCrops
            .Where(fc => fc.FieldId == id && fc.IsActive)
            .ProjectTo<FieldCropViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(fieldCrops);
    }
    
    [HttpGet("{id}", Name = nameof(GetFarmFieldCrops))]
    public async Task<IActionResult> GetFarmFieldCrops(string id)
    {
        var fieldCrops = await _context.FieldCrops
            .Where(fc => fc.Field.FarmId == id && fc.IsActive)
            .ProjectTo<FieldCropViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(fieldCrops);
    }

    [HttpGet("{id}", Name = nameof(GetFieldCrop))]
    public async Task<IActionResult> GetFieldCrop(string id)
    {
        var fieldCrop = await _context.FieldCrops
            .ProjectTo<FieldCropViewModel>(_configuration)
            .FirstOrDefaultAsync(fc => fc.Id == id)
            .ConfigureAwait(false);

        return fieldCrop == null 
            ? Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Field crop does not exist")) 
            : Ok(new ApiOkResponse(fieldCrop));
    }

    [HttpPost(Name = nameof(CreateFieldCrop))]
    public async Task<IActionResult> CreateFieldCrop([FromBody] FieldCropViewModel model)
    {
        try
        {
            var fieldCrop = _configuration.CreateMapper().Map<FieldCrop>(model);

            await _context.AddAsync(fieldCrop).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            var newFieldCrop = await _context.FieldCrops
                .Where(fc => fc.Id == fieldCrop.Id)
                .ProjectTo<FieldCropViewModel>(_configuration)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return Ok(new ApiOkResponse(newFieldCrop, "Field crop successfully created"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating field crop");
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, "Error creating field crop"));
        }
    }
    
    [HttpPut("{id}", Name = nameof(UpdateFieldCrop))]
    public async Task<IActionResult> UpdateFieldCrop(string id, [FromBody] FieldCropViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var fieldCrop = await _context.FieldCrops.FirstOrDefaultAsync(fc => fc.Id == id).ConfigureAwait(false);
        if (fieldCrop == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Field crop not found"));
        }

        fieldCrop.CropId = model.CropId;
        fieldCrop.FieldId = model.FieldId;
        fieldCrop.PlantedDate = model.PlantedDate;
        fieldCrop.HarvestDate = model.HarvestDate;
        fieldCrop.Size = model.Size;
        fieldCrop.Irrigation = model.Irrigation;

        _context.Entry(fieldCrop).State = EntityState.Modified;
        await _context.SaveChangesAsync().ConfigureAwait(false);

        var updatedFieldCrop = await _context.FieldCrops
            .Where(fc => fc.Id == id)
            .ProjectTo<FieldCropViewModel>(_configuration)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return Ok(new ApiOkResponse(updatedFieldCrop, "Field crop successfully updated"));
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteFieldCrop))]
    public async Task<IActionResult> DeleteFieldCrop(string id)
    {
        var fieldCrop = await _context.FieldCrops.FirstOrDefaultAsync(fc => fc.Id == id).ConfigureAwait(false);

        if (fieldCrop == null) return NotFound();

        fieldCrop.Deleted = true;
        fieldCrop.IsActive = false;

        _context.Entry(fieldCrop).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return Ok(new ApiOkResponse(id, "Field crop was deleted successfully"));
    }
    
    [HttpGet(Name = nameof(GetCurrentSeasonCrops))]
    public async Task<IActionResult> GetCurrentSeasonCrops()
    {
        // Assuming current season is based on current year
        var currentYear = DateTime.Now.Year;
        var startDate = new DateTime(currentYear, 1, 1);
        var endDate = new DateTime(currentYear, 12, 31);
        
        var fieldCrops = await _context.FieldCrops
            .Where(fc => fc.IsActive && fc.PlantedDate >= startDate && fc.PlantedDate <= endDate)
            .ProjectTo<FieldCropViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(fieldCrops);
    }
    
    [HttpGet("{id}", Name = nameof(GetFarmCurrentSeasonCrops))]
    public async Task<IActionResult> GetFarmCurrentSeasonCrops(string id)
    {
        // Assuming current season is based on current year
        var currentYear = DateTime.Now.Year;
        var startDate = new DateTime(currentYear, 1, 1);
        var endDate = new DateTime(currentYear, 12, 31);
        
        var fieldCrops = await _context.FieldCrops
            .Where(fc => fc.IsActive && fc.Field.FarmId == id && fc.PlantedDate >= startDate && fc.PlantedDate <= endDate)
            .ProjectTo<FieldCropViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(fieldCrops);
    }
    
    [HttpGet(Name = nameof(GetCropStatistics))]
    public async Task<IActionResult> GetCropStatistics()
    {
        // Get current year stats
        var currentYear = DateTime.Now.Year;
        var startDate = new DateTime(currentYear, 1, 1);
        var endDate = new DateTime(currentYear, 12, 31);
        
        var fieldCrops = await _context.FieldCrops
            .Where(fc => fc.IsActive && fc.PlantedDate >= startDate && fc.PlantedDate <= endDate)
            .Include(fc => fc.Crop)
            .ToListAsync()
            .ConfigureAwait(false);
        
        var statistics = fieldCrops
            .GroupBy(fc => fc.Crop.Name)
            .Select(g => new
            {
                CropName = g.Key,
                TotalHectares = g.Sum(fc => fc.Size),
                FieldCount = g.Count(),
                IrrigatedHectares = g.Where(fc => fc.Irrigation).Sum(fc => fc.Size),
                NonIrrigatedHectares = g.Where(fc => !fc.Irrigation).Sum(fc => fc.Size)
            })
            .ToList();
        
        return Ok(statistics);
    }
    
    [HttpGet("{id}", Name = nameof(GetFarmCropStatistics))]
    public async Task<IActionResult> GetFarmCropStatistics(string id)
    {
        // Get current year stats for a specific farm
        var currentYear = DateTime.Now.Year;
        var startDate = new DateTime(currentYear, 1, 1);
        var endDate = new DateTime(currentYear, 12, 31);
        
        var fieldCrops = await _context.FieldCrops
            .Where(fc => fc.IsActive && fc.Field.FarmId == id && fc.PlantedDate >= startDate && fc.PlantedDate <= endDate)
            .Include(fc => fc.Crop)
            .ToListAsync()
            .ConfigureAwait(false);
        
        var statistics = fieldCrops
            .GroupBy(fc => fc.Crop.Name)
            .Select(g => new
            {
                CropName = g.Key,
                TotalHectares = g.Sum(fc => fc.Size),
                FieldCount = g.Count(),
                IrrigatedHectares = g.Where(fc => fc.Irrigation).Sum(fc => fc.Size),
                NonIrrigatedHectares = g.Where(fc => !fc.Irrigation).Sum(fc => fc.Size)
            })
            .ToList();
        
        return Ok(statistics);
    }
}