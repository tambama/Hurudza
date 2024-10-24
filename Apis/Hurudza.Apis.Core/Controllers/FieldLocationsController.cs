using System.Net;
using System.Net.Mime;
using Asp.Versioning;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Common;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharpKml.Dom;
using SharpKml.Engine;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FieldLocationsController : Controller
{
    private readonly HurudzaDbContext _context;
    private readonly AutoMapper.IConfigurationProvider _configuration;

    public FieldLocationsController(HurudzaDbContext context, AutoMapper.IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet(Name = nameof(GetFieldLocations))]
    public async Task<IActionResult> GetFieldLocations()
    {
        var locations = await _context.Locations.OfType<FieldLocation>().OrderBy(l => l.CreatedDate).ToListAsync().ConfigureAwait(false);

        return Ok(locations);
    }

    [HttpGet("{id}", Name = nameof(GetFieldLocationsByFieldId))]
    public async Task<IActionResult> GetFieldLocationsByFieldId(string id)
    {
        var locations = await _context.Locations.OfType<FieldLocation>()
            .ProjectTo<FieldLocationViewModel>(_configuration)
            .Where(l => l.FieldId == id).OrderBy(l => l.CreatedDate).ToListAsync().ConfigureAwait(false);

        return Ok(locations);
    }

    [HttpGet("{id}", Name = nameof(GetFieldLocation))]
    public async Task<IActionResult> GetFieldLocation(string id)
    {
        var location = await _context.Locations.OfType<FieldLocation>().OrderBy(l => l.CreatedDate).FirstOrDefaultAsync(l => l.Id == id)
            .ConfigureAwait(false);

        if (location == null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    public async Task<IActionResult> CreateFieldLocation([FromBody] FieldLocationViewModel model)
    {
        var fieldLocation = _configuration.CreateMapper().Map<FieldLocation>(model);
        await _context.AddAsync(fieldLocation).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(model, "Location added successfully"));
    }

    [HttpPut("{id}", Name = nameof(UpdateFieldLocation))]
    public async Task<IActionResult> UpdateFieldLocation(string id, [FromBody] FieldLocation model)
    {
        var location = await _context.Locations.OfType<FieldLocation>().OrderBy(l => l.CreatedDate).FirstOrDefaultAsync(l => l.Id == model.Id)
            .ConfigureAwait(false);

        if (location == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Location does not exist"));
        }

        _context.Update(location);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(model, "Location updated successfully"));
    }

    [HttpDelete("{id}", Name = nameof(DeleteFieldLocation))]
    public async Task<IActionResult> DeleteFieldLocation(string id)
    {
        var location = await _context.Locations.OfType<FieldLocation>().OrderBy(l => l.CreatedDate).FirstOrDefaultAsync(l => l.Id == id)
            .ConfigureAwait(false);

        if (location == null)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Location does not exist"));
        }

        _context.Remove(location);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(location.Id, "Location deleted successfully"));
    }

    [HttpPost(Name = nameof(UploadKmlData))]
    public async Task<IActionResult> UploadKmlData([FromBody] FileViewModel fileViewModel)
    {
        try
        {
            await using var stream = new MemoryStream(fileViewModel.Data);
            stream.Seek(0, SeekOrigin.Begin);

            KmlFile file = KmlFile.Load(stream);
            Kml? kml = file.Root as Kml;
            if (kml != null)
            {
                foreach (var placemark in kml.Flatten().OfType<Placemark>())
                {
                    Log.Error($"{placemark.Name}");
                    if (placemark?.Geometry?.GetType() != typeof(Polygon))
                    {
                        continue;
                    }

                    var polygon = (Polygon)placemark.Geometry;
                    var outerBoundary = polygon.OuterBoundary;

                    if (outerBoundary == null)
                    {
                        continue;
                    }

                    if (placemark.Name.ToLower().Contains("farm"))
                    {
                        var coordinates = outerBoundary?.LinearRing.Coordinates.Select(c => new FarmLocation()
                        {
                            FarmId = fileViewModel.FarmId,
                            Latitude = c.Latitude,
                            Longitude = c.Longitude,
                            Altitude = c.Altitude ?? 0
                        });

                        foreach (var coordinate in coordinates)
                        {
                            await _context.AddAsync(coordinate).ConfigureAwait(false);
                            await _context.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        var field = new Field
                        {
                            Name = placemark.Name,
                            Description = string.Empty,
                            FarmId = fileViewModel.FarmId,
                        };

                        await _context.AddAsync(field).ConfigureAwait(false);

                        var coordinates = outerBoundary?.LinearRing.Coordinates.Select(c => new FieldLocation()
                        {
                            FarmId = fileViewModel.FarmId,
                            FieldId = field.Id,
                            Latitude = c.Latitude,
                            Longitude = c.Longitude,
                            Altitude = c.Altitude ?? 0
                        });

                        foreach (var coordinate in coordinates)
                        {
                            await _context.AddAsync(coordinate).ConfigureAwait(false);
                            await _context.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                }

                return Ok(new ApiResponse((int)HttpStatusCode.OK, "Successfully Imported Data"));
            }
            else
            {
                return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Failed to read file. Try again"));
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message, e);
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Failed to read file. Try again"));
        }
    }
}