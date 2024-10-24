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
public class EntitiesController : Controller
{
    private readonly HurudzaDbContext _context;
    private readonly IConfigurationProvider _configuration;

    public EntitiesController(HurudzaDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    [HttpGet(Name = nameof(GetEntities))]
    public async Task<IActionResult> GetEntities()
    {
        var entities = await _context.Entities.Where(e => e.IsActive).ToListAsync().ConfigureAwait(false);

        return Ok(entities);
    }
    
    [HttpGet(Name = nameof(GetEntityList))]
    public async Task<IActionResult> GetEntityList()
    {
        var entities = await _context.Entities.Where(e => e.IsActive).ProjectTo<EntityListViewModel>(_configuration).ToListAsync().ConfigureAwait(false);

        return Ok(entities);
    }

    [HttpGet("{id}", Name = nameof(GetEntity))]
    public async Task<IActionResult> GetEntity(string id)
    {
        var entity = await _context.Entities.SingleOrDefaultAsync(c => c.Id == id).ConfigureAwait(false);

        return entity == null
            ? Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Entity does not exist"))
            : Ok(new ApiOkResponse(entity));
    }

    [HttpPost(Name = nameof(CreateEntity))]
    public async Task<IActionResult> CreateEntity([FromBody] EntityViewModel model)
    {
        var entity = _configuration.CreateMapper().Map<Entity>(model);
        
        await _context.Entities.AddAsync(entity).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
        return Ok(new ApiOkResponse(entity, "Created entity successfully"));
    }

    [HttpPut("{id}", Name = nameof(UpdateEntity))]
    public async Task<IActionResult> UpdateEntity(string id, [FromBody] EntityViewModel model)
    {
        try
        {
            if (id != model.Id)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Entity not found"));
            }

            var entity = await _context.Entities.FirstOrDefaultAsync(f => f.Id == id).ConfigureAwait(false);
            if (entity == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Entity not found"));
            }

            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.EntityType = model.EntityType;
            entity.PhoneNumber = model.PhoneNumber;
            entity.Email = model.Email;

            _context.Update(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Ok(new ApiOkResponse(entity, "Successfully updated entity"));
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
            return Ok(new ApiResponse((int)HttpStatusCode.InternalServerError, "Failed to update entity"));
        }
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteEntity))]
    public async Task<IActionResult> DeleteEntity(string id)
    {
        var entity = await _context.Entities.FirstOrDefaultAsync(f => f.Id == id).ConfigureAwait(false);

        if (entity == null) return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Entity not found"));

        entity.Deleted = true;
        entity.IsActive = false;

        _context.Update(entity);
        await _context.SaveChangesAsync();

        return Ok(new ApiOkResponse(entity, $"{entity.Name} was deleted successfully"));
    }
}