using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Data;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfigurationProvider _configuration;

    public FarmsController(HurudzaDbContext context, IConfigurationProvider configuration,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _configuration = configuration;
        _userManager = userManager;
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
        var farms = await _context.Farms.Where(f => f.IsActive)
            .ProjectTo<FarmListViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(farms);
    }

    [HttpGet("{id}", Name = nameof(GetFarm))]
    public async Task<IActionResult> GetFarm(string id)
    {
        var farm = await _context.Farms
            .Include(f => f.Province)
            .Include(f => f.District)
            .Include(f => f.LocalAuthority)
            .Include(f => f.Ward)
            .ProjectTo<FarmViewModel>(_configuration)
            .FirstOrDefaultAsync(f => f.Id == id)
            .ConfigureAwait(false);

        return Ok(farm == null
            ? new ApiResponse((int)HttpStatusCode.NotFound, "Farm not found")
            : new ApiOkResponse(farm));
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
        var farm = new Farm();
        farm.Name = model.Name;
        farm.Address = model.Address;
        farm.Description = model.Description;
        farm.Vision = model.Vision;
        farm.ContactPerson = model.ContactPerson;
        farm.PhoneNumber = model.PhoneNumber;
        farm.Email = model.Email;
        farm.Website = model.Website;
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
        farm.WardId = model.WardId;
        farm.LocalAuthorityId = model.LocalAuthorityId;
        farm.DistrictId = model.DistrictId;
        farm.ProvinceId = model.ProvinceId;
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
        farm.Website = model.Website;
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
        farm.WardId = model.WardId;
        farm.LocalAuthorityId = model.LocalAuthorityId;
        farm.DistrictId = model.DistrictId;
        farm.ProvinceId = model.ProvinceId;
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

        var updatedFarm = await _context.Farms.Where(f => f.Id == farm.Id).ProjectTo<FarmViewModel>(_configuration)
            .FirstOrDefaultAsync().ConfigureAwait(false);

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

    #region Schools

    private async Task<bool> UserHasAccessToFarm(string userId, string farmId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(farmId))
            return false;

        // System Administrators have access to all farms
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && await _userManager.IsInRoleAsync(user, ApiRoles.SystemAdministrator))
            return true;

        // Check if user has a profile for this farm
        return await _context.UserProfiles
            .AnyAsync(p => p.UserId == userId && p.FarmId == farmId && p.IsActive);
    }

    [HttpGet("schools", Name = nameof(GetSchools))]
    public async Task<IActionResult> GetSchools()
    {
        var schools = await _context.Farms
            .Where(f => f.FarmType == FarmType.School)
            .ProjectTo<FarmViewModel>(_configuration)
            .ToListAsync();

        return Ok(new ApiOkResponse(schools));
    }

    [HttpGet("school/{schoolId}/farms", Name = nameof(GetSchoolFarms))]
    [Authorize(Policy = "IsFarmManager")]
    public async Task<IActionResult> GetSchoolFarms(string schoolId)
    {
        var farms = await _context.Farms
            .Where(f => f.ParentSchoolId == schoolId && f.FarmType == FarmType.Farm)
            .ProjectTo<FarmViewModel>(_configuration)
            .ToListAsync();

        return Ok(new ApiOkResponse(farms));
    }

    [HttpPost("school/{schoolId}/add-farm", Name = nameof(AddFarmToSchool))]
    [Authorize(Policy = "IsAdministrator")]
    public async Task<IActionResult> AddFarmToSchool(string schoolId, [FromBody] AddFarmViewModel model)
    {
        // Verify the user has access to this school
        var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
        if (!await UserHasAccessToFarm(currentUserId, schoolId))
        {
            return Forbid();
        }

        // Get the parent school to inherit properties
        var parentSchool = await _context.Farms
            .FirstOrDefaultAsync(f => f.Id == schoolId && f.FarmType == FarmType.School);

        if (parentSchool == null)
        {
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Parent school not found"));
        }

        var farm = new Farm
        {
            Name = model.Name,
            Address = model.Address,
            ContactPerson = model.ContactPerson,
            PhoneNumber = model.PhoneNumber,
            Size = model.Size,
            Arable = model.Arable,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            FarmType = FarmType.Farm,
            ParentSchoolId = schoolId,
            RequiresTillageService = model.RequiresTillageService,
            TillageRequirements = model.TillageRequirements,
            CropRotationPlan = model.CropRotationPlan,
            Email = $"farm_{Guid.NewGuid().ToString().Substring(0, 8)}@tillage.local",
            
            // Inherit properties from parent school
            ProvinceId = parentSchool.ProvinceId,
            DistrictId = parentSchool.DistrictId,
            LocalAuthorityId = parentSchool.LocalAuthorityId,
            WardId = parentSchool.WardId,
            Region = parentSchool.Region,
            Conference = parentSchool.Conference,
            
            // Set reasonable defaults for farm-specific properties
            WaterAvailability = WaterAvailability.Seasonal,
            RoadAccess = RoadAccess.Dust,
            Terrain = Terrain.Flat
        };

        await _context.Farms.AddAsync(farm);
        await _context.SaveChangesAsync();

        var newFarm = await _context.Farms
            .Where(f => f.Id == farm.Id)
            .ProjectTo<FarmViewModel>(_configuration)
            .FirstOrDefaultAsync();

        return Ok(new ApiOkResponse(newFarm, "Farm added successfully"));
    }

    [HttpGet("school-with-farms/{schoolId}", Name = nameof(GetSchoolWithFarms))]
    public async Task<IActionResult> GetSchoolWithFarms(string schoolId)
    {
        var school = await _context.Farms
            .Include(f => f.ManagedFarms)
            .Where(f => f.Id == schoolId && f.FarmType == FarmType.School)
            .ProjectTo<FarmViewModel>(_configuration)
            .FirstOrDefaultAsync();

        if (school == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "School not found"));

        return Ok(new ApiOkResponse(school));
    }

    #endregion
}