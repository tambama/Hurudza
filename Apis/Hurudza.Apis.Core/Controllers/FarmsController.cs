using System.Net;
using System.Net.Mime;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Enums.Enums;
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
        var farms = await _context.Farms.Where(f => f.IsActive)
            .ProjectTo<FarmViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(farms);
    }
    
    [HttpGet(Name = nameof(GetFarmList))]
    public async Task<IActionResult> GetFarmList()
    {
        var farms = await _context.Farms.Where(f =>f.IsActive)
            .ProjectTo<FarmListViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(farms);
    }

    [HttpGet("{id}", Name = nameof(GetFarmDetails))]
    public async Task<IActionResult> GetFarmDetails(string id)
    {
        var farm = await _context.Farms
            .Include(f => f.Fields)
            .ProjectTo<FarmMapViewModel>(_configuration)
            .FirstOrDefaultAsync(f => f.Id == id)
            .ConfigureAwait(false);

        return Ok(farm);
    }

    [HttpPost(Name = nameof(CreateFarm))]
    public async Task<IActionResult> CreateFarm([FromBody] FarmViewModel model)
    {
        var farm = _configuration.CreateMapper().Map<Farm>(model);

        await _context.AddAsync(farm).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        var newFarm = await _context.Farms.Where(f => f.Id == farm.Id).ProjectTo<FarmViewModel>(_configuration)
            .FirstOrDefaultAsync();

        return Ok(new ApiOkResponse(newFarm, "Farm successfully created"));
    }
    
    [HttpPut("{id}", Name = nameof(UpdateFarm))]
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

        farm.Name = model.Name;
        farm.Address = model.Address;
        farm.Description = model.Description;
        farm.Vision = model.Vision;
        farm.ContactPerson = model.ContactPerson;
        farm.PhoneNumber = model.PhoneNumber;
        farm.Email = model.Email;
        farm.Website = model.Website ;
        farm.Year = model.Year;
        farm.FoundingMembers = model.FoundingMembers;
        farm.KeyMilestones = model.KeyMilestones;
        farm.Size = model.Size;
        farm.Arable = model.Arable;
        farm.Cleared = model.Cleared;
        farm.PastCrops = model.PastCrops;
        farm.LandManagementPractice = model.LandManagementPractice;
        farm.SecurityMeasures = model.SecurityMeasures;
        farm.WaterSource = model.WaterSource;
        farm.WaterAvailability = model.WaterAvailability ?? WaterAvailability.Seasonal;
        farm.Irrigated = model.Irrigated;
        farm.Equipment = model.Equipment;
        farm.SoilType = model.SoilType;
        farm.RoadAccess = model.RoadAccess ?? RoadAccess.Dust;
        farm.Problems = model.Problems;
        farm.Personnel = model.Personnel;
        farm.Recommendations = model.Recommendations;
        farm.WardId  = model.WardId;
        farm.LocalAuthorityId  = model.LocalAuthorityId;
        farm.DistrictId  = model.DistrictId;
        farm.Region = model.Region ?? Region.I;
        farm.Conference = model.Conference ?? Conference.East;
        farm.CommunityPrograms = model.CommunityPrograms;
        farm.Partnerships = model.Partnerships;
        farm.Classrooms = model.Classrooms;
        farm.Laboratories = model.Laboratories;
        farm.SportsFacilities = model.SportsFacilities;
        farm.Library = model.Library;
        farm.AverageExamScores = model.AverageExamScores;
        farm.ExtraCurricularAchievements = model.ExtraCurricularAchievements;
        farm.MaleTeachers = model.MaleTeachers;
        farm.FemaleTeachers = model.FemaleTeachers;
        farm.MaleStudents = model.MaleStudents;
        farm.FemaleStudents = model.FemaleStudents;
        farm.StudentDemographics = model.StudentDemographics;
        farm.DayScholars = model.DayScholars;
        farm.BoardingScholars = model.BoardingScholars;
        farm.NonTeachingStaff = model.NonTeachingStaff;
        farm.Terrain = model.Terrain ?? Terrain.Flat;
        farm.Latitude = model.Latitude;
        farm.Longitude = model.Longitude;
        farm.Elevation = model.Elevation;
        farm.Buildings = model.Buildings;

        _context.Update(farm);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
        var updatedFarm = await _context.Farms.Where(f => f.Id == farm.Id).ProjectTo<FarmViewModel>(_configuration).FirstOrDefaultAsync().ConfigureAwait(false);

        return Ok(new ApiOkResponse(updatedFarm, $"{farm.Name} successfully updated"));
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