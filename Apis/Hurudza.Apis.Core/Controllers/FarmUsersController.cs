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
    [HttpGet(Name = nameof(GetFarmUsers))]
    [Authorize(Policy = "CanViewUsers")]
    public async Task<IActionResult> GetFarmUsers([FromQuery] string farmId)
    {
        try
        {
            // Verify the user has access to this farm
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));

            // Check access to farm (unless System Administrator)
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(userId, farmId))
                return Forbid();

            var farmUsers = await _farmUserManagerService.GetFarmUsersAsync(farmId);
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
    [HttpGet(Name = nameof(GetFarmUserSummary))]
    [Authorize(Policy = "CanViewUsers")]
    public async Task<IActionResult> GetFarmUserSummary([FromQuery] string farmId)
    {
        try
        {
            // Verify the user has access to this farm
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));

            // Check access to farm (unless System Administrator)
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(userId, farmId))
                return Forbid();

            var summary = await _farmUserManagerService.GetFarmUserSummaryAsync(farmId);
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
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving farm user summary: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets all farm assignments for a specific user
    /// </summary>
    [HttpGet(Name = nameof(GetUserFarmProfiles))]
    public async Task<IActionResult> GetUserFarmProfiles([FromQuery] string userId)
    {
        try
        {
            // If no userId is provided, use the current user
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");

            // Only System Administrators or the user themselves can see their profiles
            if (userId != User.FindFirstValue(ClaimTypes.PrimarySid) && 
                !User.IsInRole(ApiRoles.SystemAdministrator))
                return Forbid();

            var profiles = await _farmUserManagerService.GetUserFarmProfilesAsync(userId);
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
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving user profiles: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets detailed access information for a specific user
    /// </summary>
    [HttpGet(Name = nameof(GetUserFarmAccess))]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<IActionResult> GetUserFarmAccess([FromQuery] string userId)
    {
        try
        {
            // If no userId is provided, use the current user
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");

            var userAccess = await _farmUserManagerService.GetUserFarmAccessAsync(userId);
            return Ok(new ApiOkResponse(userAccess));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found error in GetUserFarmAccess");
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserFarmAccess");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving user farm access: {ex.Message}"));
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
            if (model == null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid request data"));

            // Verify the user has access to this farm if they're not a System Administrator
            var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(currentUserId, model.FarmId))
                return Forbid();

            // Only System Administrators can assign System Administrator role
            if (model.Role == ApiRoles.SystemAdministrator && !User.IsInRole(ApiRoles.SystemAdministrator))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Forbidden, "Only System Administrators can assign System Administrator role"));

            // Make sure the role is allowed for farm assignments
            if (!IsValidFarmRole(model.Role))
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, $"Role '{model.Role}' is not valid for farm assignments"));

            var profile = await _farmUserManagerService.AssignUserToFarmAsync(
                model.UserId, model.FarmId, model.Role);

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
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error assigning user to farm: {ex.Message}"));
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
            if (model == null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid request data"));

            // Get the profile to find the farm ID
            var profile = await _context.UserProfiles.FindAsync(model.ProfileId);
            if (profile == null)
                return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Profile not found"));

            // Verify the user has access to this farm if they're not a System Administrator
            var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(currentUserId, profile.FarmId))
                return Forbid();

            // Only System Administrators can assign System Administrator role
            if (model.Role == ApiRoles.SystemAdministrator && !User.IsInRole(ApiRoles.SystemAdministrator))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Forbidden, "Only System Administrators can assign System Administrator role"));

            // Make sure the role is allowed for farm assignments
            if (!IsValidFarmRole(model.Role))
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, $"Role '{model.Role}' is not valid for farm assignments"));

            var updatedProfile = await _farmUserManagerService.UpdateUserFarmRoleAsync(
                model.ProfileId, model.Role, model.IsActive);

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
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error updating user farm role: {ex.Message}"));
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
            // Get the profile to find the farm ID
            var profile = await _context.UserProfiles.FindAsync(profileId);
            if (profile == null)
                return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Profile not found"));

            // Verify the user has access to this farm if they're not a System Administrator
            var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(currentUserId, profile.FarmId))
                return Forbid();

            var updatedProfile = await _farmUserManagerService.UpdateUserFarmStatusAsync(profileId, isActive);

            return Ok(new ApiOkResponse(updatedProfile, $"User farm status {(isActive ? "activated" : "deactivated")} successfully"));
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
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error updating user farm status: {ex.Message}"));
        }
    }

    /// <summary>
    /// Removes a user from a farm
    /// </summary>
    [HttpDelete(Name = nameof(RemoveUserFromFarm))]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<IActionResult> RemoveUserFromFarm(string userId, [FromQuery] string farmId)
    {
        try
        {
            // Verify the user has access to this farm if they're not a System Administrator
            var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");
            if (!User.IsInRole(ApiRoles.SystemAdministrator) && !await UserHasAccessToFarm(currentUserId, farmId))
                return Forbid();

            // Administrator cannot remove SystemAdministrator
            if (!User.IsInRole(ApiRoles.SystemAdministrator))
            {
                var userToRemove = await _userManager.FindByIdAsync(userId);
                if (userToRemove != null && await _userManager.IsInRoleAsync(userToRemove, ApiRoles.SystemAdministrator))
                    return Ok(new ApiResponse((int)HttpStatusCode.Forbidden, "Cannot remove System Administrator"));
            }

            await _farmUserManagerService.RemoveUserFromFarmAsync(userId, farmId);
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
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error removing user from farm: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets all farms that a user can access
    /// </summary>
    [HttpGet(Name = nameof(GetAccessibleFarms))]
    public async Task<IActionResult> GetAccessibleFarms([FromQuery] string userId = null)
    {
        try
        {
            // If no userId provided, use the current user
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));

            // Only allow viewing other users' accessible farms if you're an admin
            if (userId != (User.FindFirstValue(ClaimTypes.PrimarySid) ?? User.FindFirstValue("UserId")) && 
                !User.IsInRole(ApiRoles.SystemAdministrator) && 
                !User.IsInRole(ApiRoles.Administrator))
            {
                return Forbid();
            }

            var farmIds = await _farmUserManagerService.GetUserAccessibleFarmIdsAsync(userId);
            
            // Get farm details for these IDs
            var farms = await _context.Farms
                .Where(f => farmIds.Contains(f.Id) && f.IsActive && !f.Deleted)
                .Select(f => new { f.Id, f.Name })
                .ToListAsync();

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
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving accessible farms: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets all roles that can be assigned to farm users
    /// </summary>
    [HttpGet(Name = nameof(GetAvailableFarmRoles))]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<IActionResult> GetAvailableFarmRoles()
    {
        try
        {
            // Get all roles except SystemAdministrator (unless the current user is a System Administrator)
            var roles = await _context.Roles
                .Where(r => User.IsInRole(ApiRoles.SystemAdministrator) || r.Name != ApiRoles.SystemAdministrator)
                .Where(r => r.RoleClass == Hurudza.Data.Enums.Enums.RoleClass.Farm)
                .Select(r => new { r.Id, r.Name, r.Description })
                .ToListAsync();

            return Ok(new ApiOkResponse(roles));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAvailableFarmRoles");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving available roles: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets system-wide statistics about user assignments
    /// </summary>
    [HttpGet(Name = nameof(GetUserAssignmentStatistics))]
    [Authorize(Roles = "SystemAdministrator")]
    public async Task<IActionResult> GetUserAssignmentStatistics()
    {
        try
        {
            var statistics = await _farmUserManagerService.GetSystemUserAssignmentStatisticsAsync();
            return Ok(new ApiOkResponse(statistics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserAssignmentStatistics");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving user assignment statistics: {ex.Message}"));
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
        var farmRoles = new[] { 
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