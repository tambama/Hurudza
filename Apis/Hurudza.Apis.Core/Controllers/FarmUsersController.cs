using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using Asp.Versioning;
using Hurudza.Apis.Base.Models;
using Hurudza.Common.Services;
using Hurudza.Common.Utils.Exceptions;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;

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
    private readonly IFarmUserAssignmentService _farmUserAssignmentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public FarmUsersController(
        HurudzaDbContext context,
        IFarmUserAssignmentService farmUserAssignmentService,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _farmUserAssignmentService = farmUserAssignmentService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet(Name = nameof(GetFarmUsers))]
    public async Task<IActionResult> GetFarmUsers(string farmId)
    {
        try
        {
            // Verify the user has access to this farm
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));

            // Check access to farm
            if (!await UserHasAccessToFarm(userId, farmId))
                return Forbid();

            var farmUsers = await _farmUserAssignmentService.GetFarmUsersAsync(farmId);
            return Ok(new ApiOkResponse(farmUsers));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving farm users: {ex.Message}"));
        }
    }

    [HttpGet(Name = nameof(GetUserFarmProfiles))]
    public async Task<IActionResult> GetUserFarmProfiles(string userId)
    {
        try
        {
            // If no userId is provided, use the current user
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.PrimarySid);

            // Only System Administrators or the user themselves can see their profiles
            if (userId != User.FindFirstValue(ClaimTypes.PrimarySid) && 
                !User.IsInRole("SystemAdministrator"))
                return Forbid();

            var profiles = await _farmUserAssignmentService.GetUserFarmProfilesAsync(userId);
            return Ok(new ApiOkResponse(profiles));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving user profiles: {ex.Message}"));
        }
    }

    [HttpPost(Name = nameof(AssignUserToFarm))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> AssignUserToFarm([FromBody] FarmUserAssignmentViewModel model)
    {
        try
        {
            if (model == null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid request data"));

            // Verify the user has access to this farm if they're not a System Administrator
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid);
            if (!User.IsInRole("SystemAdministrator") && !await UserHasAccessToFarm(userId, model.FarmId))
                return Forbid();

            // Only System Administrators can assign System Administrator role
            if (model.Role == "SystemAdministrator" && !User.IsInRole("SystemAdministrator"))
                return Forbid();

            // Make sure the role is allowed for farm assignments
            if (!IsValidFarmRole(model.Role))
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, $"Role '{model.Role}' is not valid for farm assignments"));

            var profile = await _farmUserAssignmentService.AssignUserToFarmAsync(
                model.UserId, model.FarmId, model.Role);

            return Ok(new ApiOkResponse(profile, "User assigned to farm successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error assigning user to farm: {ex.Message}"));
        }
    }

    [HttpPut(Name = nameof(UpdateUserFarmRole))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> UpdateUserFarmRole([FromBody] UpdateFarmUserRoleViewModel model)
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
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid);
            if (!User.IsInRole("SystemAdministrator") && !await UserHasAccessToFarm(userId, profile.FarmId))
                return Forbid();

            // Only System Administrators can assign System Administrator role
            if (model.NewRole == "SystemAdministrator" && !User.IsInRole("SystemAdministrator"))
                return Forbid();

            // Make sure the role is allowed for farm assignments
            if (!IsValidFarmRole(model.NewRole))
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, $"Role '{model.NewRole}' is not valid for farm assignments"));

            var updatedProfile = await _farmUserAssignmentService.UpdateUserFarmRoleAsync(
                model.ProfileId, model.NewRole);

            return Ok(new ApiOkResponse(updatedProfile, "User farm role updated successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error updating user farm role: {ex.Message}"));
        }
    }

    [HttpDelete(Name = nameof(RemoveUserFromFarm))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> RemoveUserFromFarm(string userId, string farmId)
    {
        try
        {
            // Verify the user has access to this farm if they're not a System Administrator
            var currentUserId = User.FindFirstValue(ClaimTypes.PrimarySid);
            if (!User.IsInRole("SystemAdministrator") && !await UserHasAccessToFarm(currentUserId, farmId))
                return Forbid();

            // Administrator cannot remove SystemAdministrator
            if (!User.IsInRole("SystemAdministrator"))
            {
                var userToRemove = await _userManager.FindByIdAsync(userId);
                if (userToRemove != null && await _userManager.IsInRoleAsync(userToRemove, "SystemAdministrator"))
                    return Forbid();
            }

            await _farmUserAssignmentService.RemoveUserFromFarmAsync(userId, farmId);
            return Ok(new ApiOkResponse(null, "User removed from farm successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error removing user from farm: {ex.Message}"));
        }
    }

    [HttpGet(Name = nameof(GetAccessibleFarms))]
    public async Task<IActionResult> GetAccessibleFarms()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));

            var farmIds = await _farmUserAssignmentService.GetUserAccessibleFarmIdsAsync(userId);
            
            // Get farm details for these IDs
            var farms = await _context.Farms
                .Where(f => farmIds.Contains(f.Id) && f.IsActive && !f.Deleted)
                .Select(f => new { f.Id, f.Name })
                .ToListAsync();

            return Ok(new ApiOkResponse(farms));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving accessible farms: {ex.Message}"));
        }
    }

    [HttpGet(Name = nameof(GetAvailableFarmRoles))]
    public async Task<IActionResult> GetAvailableFarmRoles()
    {
        try
        {
            // Get all roles except SystemAdministrator (unless the current user is a System Administrator)
            var roles = await _context.Roles
                .Where(r => User.IsInRole("SystemAdministrator") || r.Name != "SystemAdministrator")
                .Where(r => r.RoleClass == Hurudza.Data.Enums.Enums.RoleClass.Farm)
                .Select(r => new { r.Id, r.Name, r.Description })
                .ToListAsync();

            return Ok(new ApiOkResponse(roles));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse((int)HttpStatusCode.InternalServerError, $"Error retrieving available roles: {ex.Message}"));
        }
    }

    #region Helper Methods

    private async Task<bool> UserHasAccessToFarm(string userId, string farmId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(farmId))
            return false;

        // System Administrators have access to all farms
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && await _userManager.IsInRoleAsync(user, "SystemAdministrator"))
            return true;

        // Check if user has a profile for this farm
        return await _context.UserProfiles
            .AnyAsync(p => p.UserId == userId && p.FarmId == farmId);
    }

    private bool IsValidFarmRole(string role)
    {
        // Define roles that can be assigned to farms
        // This could also be retrieved from the database based on RoleClass
        var validRoles = new[] { "SystemAdministrator", "Administrator", "FarmManager", "FieldOfficer", "Viewer" };
        return validRoles.Contains(role);
    }

    #endregion
}

public class FarmUserAssignmentViewModel
{
    public required string UserId { get; set; }
    public required string FarmId { get; set; }
    public required string Role { get; set; }
}

public class UpdateFarmUserRoleViewModel
{
    public required string ProfileId { get; set; }
    public required string NewRole { get; set; }
}