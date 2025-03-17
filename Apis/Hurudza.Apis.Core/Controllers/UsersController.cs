using System.Net;
using System.Net.Mime;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class UsersController : ControllerBase
{
    private readonly HurudzaDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfigurationProvider _configuration;

    public UsersController(
        HurudzaDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfigurationProvider configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpGet(Name = nameof(GetUsers))]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Where(u => u.IsActive)
            .ProjectTo<UserViewModel>(_configuration)
            .ToListAsync();

        // Load roles for each user
        foreach (var user in users)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            if (appUser != null)
            {
                var roles = await _userManager.GetRolesAsync(appUser);
                user.Role = roles.FirstOrDefault();
            }
        }

        return Ok(users);
    }

    [HttpGet("{id}", Name = nameof(GetUser))]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _context.Users
            .ProjectTo<UserViewModel>(_configuration)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) 
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "User not found"));

        // Load user roles
        var appUser = await _userManager.FindByIdAsync(id);
        if (appUser != null)
        {
            var roles = await _userManager.GetRolesAsync(appUser);
            user.Role = roles.FirstOrDefault();
        }

        // Load user profiles
        var profiles = await _context.UserProfiles
            .Where(p => p.UserId == id)
            .ProjectTo<UserProfileViewModel>(_configuration)
            .ToListAsync();

        user.Profiles = profiles;

        return Ok(new ApiOkResponse(user));
    }
    
    [HttpGet("{username}", Name = nameof(GetUserByUsername))]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await _context.Users
            .ProjectTo<UserViewModel>(_configuration)
            .FirstOrDefaultAsync(u => u.UserName == username);

        if (user == null) 
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "User not found"));

        // Load user roles
        var appUser = await _userManager.FindByNameAsync(username);
        if (appUser != null)
        {
            var roles = await _userManager.GetRolesAsync(appUser);
            user.Role = roles.FirstOrDefault();
        }

        return Ok(new ApiOkResponse(user));
    }

    [HttpGet("{farmId}", Name = nameof(GetFarmUsers))]
    public async Task<IActionResult> GetFarmUsers(string farmId)
    {
        var userProfiles = await _context.UserProfiles
            .Where(p => p.FarmId == farmId && p.IsActive)
            .ToListAsync();

        var users = new List<UserViewModel>();
        foreach (var profile in userProfiles)
        {
            var user = await _userManager.FindByIdAsync(profile.UserId);
            if (user != null && user.IsActive)
            {
                var userViewModel = _configuration.CreateMapper().Map<UserViewModel>(user);
                userViewModel.Role = profile.Role;
                userViewModel.FarmId = farmId;
                userViewModel.ProfileId = profile.Id;
                
                users.Add(userViewModel);
            }
        }

        return Ok(users);
    }

    [HttpPost(Name = nameof(CreateUser))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> CreateUser([FromBody] UserViewModel model)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByNameAsync(model.Email);
        if (existingUser != null)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "User with this email already exists"));

        // Create new user
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Firstname = model.Firstname,
            Surname = model.Surname,
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = true // Auto-confirm email for admin-created users
        };

        var result = await _userManager.CreateAsync(user, model.Password ?? "Password+1");
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                "Failed to create user: " + string.Join(", ", errors)));
        }

        // Assign role
        if (!string.IsNullOrEmpty(model.Role))
        {
            result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
            {
                await _userManager.DeleteAsync(user); // Rollback user creation
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                    "Failed to assign role: " + string.Join(", ", errors)));
            }
        }

        // Create user profile if farm ID is provided
        if (!string.IsNullOrEmpty(model.FarmId))
        {
            var profile = new UserProfile
            {
                UserId = user.Id,
                FarmId = model.FarmId,
                Role = model.Role ?? "User" // Default role if none provided
            };

            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        var createdUser = await _context.Users
            .ProjectTo<UserViewModel>(_configuration)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        return Ok(new ApiOkResponse(createdUser, "User created successfully"));
    }

    [HttpPut("{id}", Name = nameof(UpdateUser))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserViewModel model)
    {
        if (id != model.Id)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "User ID mismatch"));

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "User not found"));

        // Update basic properties
        user.Firstname = model.Firstname;
        user.Surname = model.Surname;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.UserName = model.Email; // Keep username and email in sync

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                "Failed to update user: " + string.Join(", ", errors)));
        }

        // Update password if provided
        if (!string.IsNullOrEmpty(model.Password))
        {
            // First remove existing password
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (removePasswordResult.Succeeded)
            {
                // Then add new password
                result = await _userManager.AddPasswordAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                        "Failed to update password: " + string.Join(", ", errors)));
                }
            }
        }

        // Update role if provided
        if (!string.IsNullOrEmpty(model.Role))
        {
            // First remove all current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                        "Failed to remove current roles: " + string.Join(", ", errors)));
                }
            }

            // Then add new role
            result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                    "Failed to assign new role: " + string.Join(", ", errors)));
            }

            // Update role in profile if ProfileId is provided
            if (!string.IsNullOrEmpty(model.ProfileId))
            {
                var profile = await _context.UserProfiles.FindAsync(model.ProfileId);
                if (profile != null)
                {
                    profile.Role = model.Role;
                    _context.UserProfiles.Update(profile);
                    await _context.SaveChangesAsync();
                }
            }
        }

        var updatedUser = await _context.Users
            .ProjectTo<UserViewModel>(_configuration)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        return Ok(new ApiOkResponse(updatedUser, "User updated successfully"));
    }

    [HttpPut("{id}", Name = nameof(UpdateUserRole))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateUserRoleViewModel model)
    {
        if (id != model.Id)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "User ID mismatch"));

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "User not found"));

        // Remove from old role if specified
        if (!string.IsNullOrEmpty(model.FromRole))
        {
            var result = await _userManager.RemoveFromRoleAsync(user, model.FromRole);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                    "Failed to remove from current role: " + string.Join(", ", errors)));
            }
        }

        // Add to new role
        var addResult = await _userManager.AddToRoleAsync(user, model.ToRole);
        if (!addResult.Succeeded)
        {
            var errors = addResult.Errors.Select(e => e.Description).ToList();
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                "Failed to assign new role: " + string.Join(", ", errors)));
        }

        // Update role in profile if farm ID is provided
        if (!string.IsNullOrEmpty(model.FarmId))
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == id && p.FarmId == model.FarmId);
            
            if (profile != null)
            {
                profile.Role = model.ToRole;
                _context.UserProfiles.Update(profile);
                await _context.SaveChangesAsync();
            }
        }

        return Ok(new ApiOkResponse(null, "User role updated successfully"));
    }

    [HttpDelete("{id}", Name = nameof(DeleteUser))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "User not found"));

        // Soft delete - mark as inactive rather than hard delete
        user.IsActive = false;
        user.IsDeleted = true;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                "Failed to delete user: " + string.Join(", ", errors)));
        }

        // Also deactivate all profiles
        var profiles = await _context.UserProfiles.Where(p => p.UserId == id).ToListAsync();
        foreach (var profile in profiles)
        {
            profile.IsActive = false;
            profile.Deleted = true;
        }
        await _context.SaveChangesAsync();

        return Ok(new ApiOkResponse(null, "User deleted successfully"));
    }

    [HttpPost(Name = nameof(ResetPassword))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "User not found"));

        // Generate password reset token
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        // Reset password
        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
        
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, 
                "Failed to reset password: " + string.Join(", ", errors)));
        }

        return Ok(new ApiOkResponse(null, "Password reset successfully"));
    }

    [HttpGet(Name = nameof(GetUserRoles))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "User not found"));

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }

    [HttpGet(Name = nameof(GetUserClaims))]
    [Authorize(Roles = "SystemAdministrator,Administrator")]
    public async Task<IActionResult> GetUserClaims(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "User not found"));

        var claims = await _userManager.GetClaimsAsync(user);
        var claimViewModels = claims.Select(c => new ClaimViewModel
        {
            ClaimType = c.Type,
            ClaimValue = c.Value
        }).ToList();

        return Ok(claimViewModels);
    }
}