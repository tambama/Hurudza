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
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FieldsController : Controller
{
    private readonly HurudzaDbContext _context;
    private readonly IConfigurationProvider _configuration;

    public FieldsController(HurudzaDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet(Name = nameof(GetFields))]
    public async Task<IActionResult> GetFields()
    {
        var fields = await _context.Fields
            .ProjectTo<FieldViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(fields);
    }

    [HttpGet("{id}", Name = nameof(GetFarmFields))]
    public async Task<IActionResult> GetFarmFields(string id)
    {
        var fields = await _context.Fields
            .Include(f => f.Locations)
            .Where(f => f.FarmId == id)
            .ProjectTo<FieldViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(fields);
    }

    [HttpGet("{id}", Name = nameof(GetField))]
    public async Task<IActionResult> GetField(string id)
    {
        var field = await _context.Fields.ProjectTo<FieldViewModel>(_configuration).FirstOrDefaultAsync(f => f.Id == id).ConfigureAwait(false);

        return Ok(field == null ? new ApiResponse((int)HttpStatusCode.NotFound, "Field does not exists") : new ApiOkResponse(field));
    }

    [HttpPost(Name = nameof(CreateField))]
    public async Task<IActionResult> CreateField([FromBody] FieldViewModel model)
    {
        var field = _configuration.CreateMapper().Map<Field>(model);

        await _context.AddAsync(field).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(field, "Field successfully created"));
    }
    
    [HttpPut("{id}", Name = nameof(UpdateField))]
    public async Task<IActionResult> UpdateField(string id, [FromBody] FieldViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var field = await _context.Fields.FirstOrDefaultAsync(f => f.Id == id).ConfigureAwait(false);
        if (field == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Resource not found"));
        }

        field.SoilType = model.SoilType;
        field.Name = model.Name;
        field.Description = model.Description;
        field.Irrigation = model.Irrigation;
        field.Size = model.Size;
        field.FarmId = model.FarmId;

        _context.Entry(field).State = EntityState.Modified;
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(field, $"{field.Name} successfully updated"));
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteField))]
    public async Task<IActionResult> DeleteField(string id)
    {
        var field = await _context.Fields.FirstOrDefaultAsync(f => f.Id == id).ConfigureAwait(false);

        if (field == null) return NotFound();

        field.Deleted = true;
        field.IsActive = false;

        _context.Entry(field).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return Ok(new ApiOkResponse(field, $"{field.Name} was deleted successfully"));
    }
}