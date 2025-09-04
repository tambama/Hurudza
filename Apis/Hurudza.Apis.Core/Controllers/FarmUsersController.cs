using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using Asp.Versioning;
using Hurudza.Apis.Base.Models;
using Hurudza.Common.Utils.Exceptions;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Data;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FarmUsersController : ControllerBase
{
    private readonly HurudzaDbContext _context;
    private readonly IFarmUserManagerService _farmUserManagerService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<FarmUsersController> _logger;

    public FarmUsersController(
        HurudzaDbContext context,
        IFarmUserManagerService farmUserManagerService,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<FarmUsersController> logger)
    {
        _context = context;
        _farmUserManagerService = farmUserManagerService;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Gets all users assigned to a specific farm
    /// </summary>
    [HttpGet("{farmId}", Name = nameof(GetFarmUsers))]
    [Authorize(Policy = "CanViewUsers")]
    public async Task<IActionResult> GetFarmUsers(string farmId)
    {
        try
        {
            _logger.LogInformation($"GetFarmUsers called for farmId: {farmId}");

            // Verify the user has access to this farm
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User not authenticated in GetFarmUsers");
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));
            }

            // Check access to farm (unless System Administrator)
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(userId, farmId))
            {
                _logger.LogWarning($"User {userId} does not have access to farm {farmId}");
                return Forbid();
            }

            var farmUsers = await _farmUserManagerService.GetFarmUsersAsync(farmId);
            _logger.LogInformation($"Retrieved {farmUsers.Count} users for farm {farmId}");
            return Ok(new ApiOkResponse(farmUsers));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in GetFarmUsers");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetFarmUsers");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving farm users: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets summary statistics about users assigned to a farm
    /// </summary>
    [HttpGet("{farmId}", Name = nameof(GetFarmUserSummary))]
    [Authorize(Policy = "CanViewUsers")]
    public async Task<IActionResult> GetFarmUserSummary(string farmId)
    {
        try
        {
            _logger.LogInformation($"GetFarmUserSummary called for farmId: {farmId}");

            // Verify the user has access to this farm
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User not authenticated in GetFarmUserSummary");
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));
            }

            // Check access to farm (unless System Administrator)
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(userId, farmId))
            {
                _logger.LogWarning($"User {userId} does not have access to farm {farmId}");
                return Forbid();
            }

            var summary = await _farmUserManagerService.GetFarmUserSummaryAsync(farmId);
            _logger.LogInformation($"Retrieved summary for farm {farmId}");
            return Ok(new ApiOkResponse(summary));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in GetFarmUserSummary");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetFarmUserSummary");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error retrieving farm user summary: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets all farm assignments for a specific user
    /// </summary>
    [HttpGet("{userId}", Name = nameof(GetUserFarmProfiles))]
    public async Task<IActionResult> GetUserFarmProfiles(string userId)
    {
        try
        {
            _logger.LogInformation($"GetUserFarmProfiles called for userId: {userId}");

            // If no userId is provided, use the current user
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");

            // Only System Administrators or the user themselves can see their profiles
            if (userId != User.FindFirstValue(ClaimTypes.PrimarySid) &&
                !User.IsInRole(ApiRoles.SystemAdministrator))
            {
                _logger.LogWarning(
                    $"User {User.FindFirstValue(ClaimTypes.PrimarySid)} attempted to access profiles for user {userId}");
                return Forbid();
            }

            var profiles = await _farmUserManagerService.GetUserFarmProfilesAsync(userId);
            _logger.LogInformation($"Retrieved {profiles.Count} profiles for user {userId}");
            return Ok(new ApiOkResponse(profiles));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in GetUserFarmProfiles");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserFarmProfiles");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error retrieving user profiles: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets all available farm roles for assignment
    /// </summary>
    [HttpGet(Name = nameof(GetAvailableFarmRoles))]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<IActionResult> GetAvailableFarmRoles()
    {
        try
        {
            _logger.LogInformation("GetAvailableFarmRoles called");

            // Get farm roles (roles with RoleClass == Farm)
            var roles = await _context.Roles
                .Where(r => User.IsInRole(ApiRoles.SystemAdministrator) || r.Name != ApiRoles.SystemAdministrator)
                .Where(r => r.RoleClass == Hurudza.Data.Enums.Enums.RoleClass.Farm)
                .Select(r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();

            _logger.LogInformation($"Retrieved {roles.Count} available farm roles");
            return Ok(new ApiOkResponse(roles));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAvailableFarmRoles");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error retrieving available roles: {ex.Message}"));
        }
    }

    /// <summary>
    /// Assigns a user to a farm with a specific role
    /// </summary>
    [HttpPost(Name = nameof(AssignUserToFarm))]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<IActionResult> AssignUserToFarm([FromBody] FarmUserAssignmentViewModel model)
    {
        try
        {
            _logger.LogInformation(
                $"AssignUserToFarm called for user {model.UserId} to farm {model.FarmId} with role {model.Role}");

            if (model == null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid request data"));

            // Verify the user has access to this farm if they're not a System Administrator
            var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(currentUserId, model.FarmId))
            {
                _logger.LogWarning($"User {currentUserId} does not have access to farm {model.FarmId}");
                return Forbid();
            }

            // Only System Administrators can assign System Administrator role
            if (model.Role == ApiRoles.SystemAdministrator && !User.IsInRole(ApiRoles.SystemAdministrator))
            {
                _logger.LogWarning($"User {currentUserId} attempted to assign SystemAdministrator role");
                return BadRequest(new ApiResponse((int)HttpStatusCode.Forbidden,
                    "Only System Administrators can assign System Administrator role"));
            }

            // Make sure the role is allowed for farm assignments
            if (!IsValidFarmRole(model.Role))
            {
                _logger.LogWarning($"Invalid farm role: {model.Role}");
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest,
                    $"Role '{model.Role}' is not valid for farm assignments"));
            }

            var profile = await _farmUserManagerService.AssignUserToFarmAsync(
                model.UserId, model.FarmId, model.Role);

            _logger.LogInformation(
                $"Successfully assigned user {model.UserId} to farm {model.FarmId} with role {model.Role}");
            return Ok(new ApiOkResponse(profile, "User assigned to farm successfully"));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in AssignUserToFarm");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AssignUserToFarm");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error assigning user to farm: {ex.Message}"));
        }
    }

    /// <summary>
    /// Updates a user's role for a specific farm
    /// </summary>
    [HttpPut(Name = nameof(UpdateUserFarmRole))]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<IActionResult> UpdateUserFarmRole([FromBody] UpdateFarmUserViewModel model)
    {
        try
        {
            _logger.LogInformation($"UpdateUserFarmRole called for profile {model.ProfileId}");

            if (model == null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid request data"));

            // Get the profile to find the farm ID
            var profile = await _context.UserProfiles.FindAsync(model.ProfileId);
            if (profile == null)
            {
                _logger.LogWarning($"Profile {model.ProfileId} not found");
                return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Profile not found"));
            }

            // Verify the user has access to this farm if they're not a System Administrator
            var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (!User.IsInRole(ApiRoles.SystemAdministrator) &&
                !await UserHasAccessToFarm(currentUserId, profile.FarmId))
            {
                _logger.LogWarning($"User {currentUserId} does not have access to farm {profile.FarmId}");
                return Forbid();
            }

            // Only System Administrators can assign System Administrator role
            if (model.Role == ApiRoles.SystemAdministrator && !User.IsInRole(ApiRoles.SystemAdministrator))
            {
                _logger.LogWarning($"User {currentUserId} attempted to assign SystemAdministrator role");
                return BadRequest(new ApiResponse((int)HttpStatusCode.Forbidden,
                    "Only System Administrators can assign System Administrator role"));
            }

            // Make sure the role is allowed for farm assignments
            if (!IsValidFarmRole(model.Role))
            {
                _logger.LogWarning($"Invalid farm role: {model.Role}");
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest,
                    $"Role '{model.Role}' is not valid for farm assignments"));
            }

            var updatedProfile = await _farmUserManagerService.UpdateUserFarmRoleAsync(
                model.ProfileId, model.Role, model.IsActive);

            _logger.LogInformation(
                $"Successfully updated profile {model.ProfileId} to role {model.Role}, active: {model.IsActive}");
            return Ok(new ApiOkResponse(updatedProfile, "User farm role updated successfully"));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in UpdateUserFarmRole");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateUserFarmRole");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error updating user farm role: {ex.Message}"));
        }
    }

    /// <summary>
    /// Updates a user's active status for a specific farm
    /// </summary>
    [HttpPut("{profileId}", Name = nameof(UpdateUserFarmStatus))]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<IActionResult> UpdateUserFarmStatus(string profileId, [FromQuery] bool isActive)
    {
        try
        {
            _logger.LogInformation($"UpdateUserFarmStatus called for profile {profileId}, status: {isActive}");

            // Get the profile to find the farm ID
            var profile = await _context.UserProfiles.FindAsync(profileId);
            if (profile == null)
            {
                _logger.LogWarning($"Profile {profileId} not found");
                return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Profile not found"));
            }

            // Verify the user has access to this farm if they're not a System Administrator
            var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (!User.IsInRole(ApiRoles.SystemAdministrator) &&
                !await UserHasAccessToFarm(currentUserId, profile.FarmId))
            {
                _logger.LogWarning($"User {currentUserId} does not have access to farm {profile.FarmId}");
                return Forbid();
            }

            var updatedProfile = await _farmUserManagerService.UpdateUserFarmStatusAsync(profileId, isActive);

            _logger.LogInformation($"Successfully updated profile {profileId} active status to {isActive}");
            return Ok(new ApiOkResponse(updatedProfile,
                $"User farm status {(isActive ? "activated" : "deactivated")} successfully"));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in UpdateUserFarmStatus");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateUserFarmStatus");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error updating user farm status: {ex.Message}"));
        }
    }

    /// <summary>
    /// Removes a user from a farm
    /// </summary>
    [HttpDelete("{userId}", Name = nameof(RemoveUserFromFarm))]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<IActionResult> RemoveUserFromFarm(string userId, [FromQuery] string farmId)
    {
        try
        {
            _logger.LogInformation($"RemoveUserFromFarm called for user {userId} from farm {farmId}");

            // Verify the user has access to this farm if they're not a System Administrator
            var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(currentUserId, farmId))
            {
                _logger.LogWarning($"User {currentUserId} does not have access to farm {farmId}");
                return Forbid();
            }

            // Administrator cannot remove SystemAdministrator
            if (!User.IsInRole(ApiRoles.SystemAdministrator))
            {
                var userToRemove = await _userManager.FindByIdAsync(userId);
                if (userToRemove != null &&
                    await _userManager.IsInRoleAsync(userToRemove, ApiRoles.SystemAdministrator))
                {
                    _logger.LogWarning($"User {currentUserId} attempted to remove a system administrator");
                    return Ok(new ApiResponse((int)HttpStatusCode.Forbidden, "Cannot remove System Administrator"));
                }
            }

            await _farmUserManagerService.RemoveUserFromFarmAsync(userId, farmId);
            _logger.LogInformation($"Successfully removed user {userId} from farm {farmId}");
            return Ok(new ApiOkResponse(null, "User removed from farm successfully"));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in RemoveUserFromFarm");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RemoveUserFromFarm");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error removing user from farm: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets all farms that a user can access
    /// </summary>
    [HttpGet("{userId}", Name = nameof(GetAccessibleFarms))]
    public async Task<IActionResult> GetAccessibleFarms(string? userId = null)
    {
        try
        {
            _logger.LogInformation($"GetAccessibleFarms called for userId: {userId}");

            // If no userId provided, use the current user
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User not authenticated in GetAccessibleFarms");
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));
            }

            // Only allow viewing other users' accessible farms if you're an admin
            if (userId != (User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId")) &&
                !User.IsInRole(ApiRoles.SystemAdministrator) &&
                !User.IsInRole(ApiRoles.Administrator))
            {
                _logger.LogWarning(
                    $"User {User.FindFirstValue(ClaimTypes.PrimarySid)} attempted to access farms for user {userId}");
                return Forbid();
            }

            var farmIds = await _farmUserManagerService.GetUserAccessibleFarmIdsAsync(userId);

            // Get farm details for these IDs
            var farms = await _context.Farms
                .Where(f => farmIds.Contains(f.Id) && f.IsActive && !f.Deleted)
                .Select(f => new FarmListViewModel { Id = f.Id, Name = f.Name })
                .ToListAsync();

            _logger.LogInformation($"Retrieved {farms.Count} accessible farms for user {userId}");
            return Ok(new ApiOkResponse(farms));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in GetAccessibleFarms");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAccessibleFarms");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error retrieving accessible farms: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets detailed farm information for all farms a user can access
    /// </summary>
    [HttpGet(Name = nameof(GetAccessibleFarmDetails))]
    public async Task<IActionResult> GetAccessibleFarmDetails(string userId = null)
    {
        try
        {
            _logger.LogInformation($"GetAccessibleFarmDetails called for userId: {userId}");

            // If no userId provided, use the current user
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User not authenticated in GetAccessibleFarmDetails");
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));
            }

            // Only allow viewing other users' accessible farms if you're an admin
            if (userId != (User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId")) &&
                !User.IsInRole(ApiRoles.SystemAdministrator) &&
                !User.IsInRole(ApiRoles.Administrator))
            {
                _logger.LogWarning(
                    $"User {User.FindFirstValue(ClaimTypes.PrimarySid)} attempted to access farms for user {userId}");
                return Forbid();
            }

            // Check if user is a System Administrator (who can access all farms)
            var user = await _userManager.FindByIdAsync(userId);
            bool isSystemAdmin = user != null && await _userManager.IsInRoleAsync(user, ApiRoles.SystemAdministrator);

            List<FarmViewModel> farms;

            if (isSystemAdmin)
            {
                // System admins can see all active farms
                farms = await _context.Farms
                    .Where(f => f.IsActive && !f.Deleted)
                    .Include(f => f.Province)
                    .Include(f => f.District)
                    .Include(f => f.LocalAuthority)
                    .Include(f => f.Ward)
                    .Select(f => new FarmViewModel
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Address = f.Address,
                        Province = f.Province != null ? f.Province.Name : null,
                        District = f.District != null ? f.District.Name : null,
                        LocalAuthority = f.LocalAuthority != null ? f.LocalAuthority.Name : null,
                        Ward = f.Ward != null ? f.Ward.Name : null,
                        Description = f.Description,
                        ContactPerson = f.ContactPerson,
                        PhoneNumber = f.PhoneNumber,
                        Email = f.Email,
                        ProvinceId = f.ProvinceId,
                        DistrictId = f.DistrictId,
                        LocalAuthorityId = f.LocalAuthorityId,
                        WardId = f.WardId,
                        Size = f.Size,
                        Latitude = f.Latitude,
                        Longitude = f.Longitude,
                        Elevation = f.Elevation,
                        Region = f.Region,
                        Conference = f.Conference,
                        FarmType = f.FarmType
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved all {farms.Count} farms for system administrator");
            }
            else
            {
                // For regular users, first get the list of farm IDs they can access
                var farmIds = await _farmUserManagerService.GetUserAccessibleFarmIdsAsync(userId);

                if (farmIds.Any())
                {
                    // Then get the details for only those farms
                    farms = await _context.Farms
                        .Where(f => farmIds.Contains(f.Id) && f.IsActive && !f.Deleted)
                        .Include(f => f.Province)
                        .Include(f => f.District)
                        .Include(f => f.LocalAuthority)
                        .Include(f => f.Ward)
                        .Select(f => new FarmViewModel
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Address = f.Address,
                            Province = f.Province != null ? f.Province.Name : null,
                            District = f.District != null ? f.District.Name : null,
                            LocalAuthority = f.LocalAuthority != null ? f.LocalAuthority.Name : null,
                            Ward = f.Ward != null ? f.Ward.Name : null,
                            Description = f.Description,
                            ContactPerson = f.ContactPerson,
                            PhoneNumber = f.PhoneNumber,
                            Email = f.Email,
                            ProvinceId = f.ProvinceId,
                            DistrictId = f.DistrictId,
                            LocalAuthorityId = f.LocalAuthorityId,
                            WardId = f.WardId,
                            Size = f.Size,
                            Latitude = f.Latitude,
                            Longitude = f.Longitude,
                            Elevation = f.Elevation,
                            Region = f.Region,
                            Conference = f.Conference,
                            FarmType = f.FarmType
                        })
                        .ToListAsync();

                    _logger.LogInformation(
                        $"Retrieved {farms.Count} farms for user {userId} with access to {farmIds.Count} farms");
                }
                else
                {
                    farms = new List<FarmViewModel>();
                    _logger.LogInformation($"User {userId} has no access to any farms");
                }
            }

            return Ok(new ApiOkResponse(farms));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in GetAccessibleFarmDetails");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAccessibleFarmDetails");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error retrieving accessible farm details: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets statistics about user assignments across all farms
    /// </summary>
    [HttpGet(Name = nameof(GetUserAssignmentStatistics))]
    [Authorize(Roles = "SystemAdministrator")]
    public async Task<IActionResult> GetUserAssignmentStatistics()
    {
        try
        {
            _logger.LogInformation("GetUserAssignmentStatistics called");

            var statistics = await _farmUserManagerService.GetSystemUserAssignmentStatisticsAsync();
            _logger.LogInformation($"Retrieved statistics for {statistics.Count} farms");
            return Ok(new ApiOkResponse(statistics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserAssignmentStatistics");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse((int)HttpStatusCode.InternalServerError,
                    $"Error retrieving user assignment statistics: {ex.Message}"));
        }
    }

    /// <summary>
    /// Checks if a user has access to a specific farm
    /// </summary>
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

    /// <summary>
    /// Checks if a role is valid for farm assignments
    /// </summary>
    private bool IsValidFarmRole(string role)
    {
        // Get farm roles from the static helper or define them here
        var farmRoles = new[]
        {
            ApiRoles.SystemAdministrator,
            ApiRoles.Administrator,
            ApiRoles.FarmManager,
            ApiRoles.FarmAdministrator,
            ApiRoles.FieldOfficer,
            ApiRoles.FarmOperator,
            ApiRoles.Viewer
        };
        return farmRoles.Contains(role);
    }
}