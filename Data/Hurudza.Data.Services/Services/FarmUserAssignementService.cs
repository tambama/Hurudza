using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Hurudza.Common.Utils.Exceptions.NotFoundException;

namespace Hurudza.Data.Services.Services;

public class FarmUserAssignmentService : IFarmUserAssignmentService
{
    private readonly HurudzaDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public FarmUserAssignmentService(
        HurudzaDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Assigns a user to a farm with a specific role
    /// </summary>
    public async Task<UserProfileViewModel> AssignUserToFarmAsync(string userId, string farmId, string role)
    {
        // Validate parameters
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        
        if (string.IsNullOrEmpty(farmId))
            throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));
        
        if (string.IsNullOrEmpty(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        // Check if user exists
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Check if farm exists
        var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == farmId);
        if (farm == null)
            throw new Hurudza.Common.Utils.Exceptions.NotFoundException("Farm not found");

        // Check if role exists
        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
            throw new NotFoundException($"Role '{role}' not found");

        // Check if the user already has this farm assignment
        var existingProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId && p.FarmId == farmId);

        if (existingProfile != null)
        {
            // Update existing profile if role changed
            if (existingProfile.Role != role)
            {
                existingProfile.Role = role;
                _context.UserProfiles.Update(existingProfile);
                await _context.SaveChangesAsync();
            }

            return new UserProfileViewModel
            {
                Id = existingProfile.Id,
                UserId = existingProfile.UserId,
                FarmId = existingProfile.FarmId,
                Farm = farm.Name,
                Role = role,
                Fullname = user.Fullname
            };
        }

        // Create new user profile
        var userProfile = new UserProfile
        {
            UserId = userId,
            FarmId = farmId,
            Role = role
        };

        await _context.UserProfiles.AddAsync(userProfile);
        await _context.SaveChangesAsync();

        // Ensure the user is assigned to the role
        await _userManager.AddToRoleAsync(user, role);

        return new UserProfileViewModel
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            FarmId = userProfile.FarmId,
            Farm = farm.Name,
            Role = role,
            Fullname = user.Fullname
        };
    }

    /// <summary>
    /// Removes a user's assignment from a farm
    /// </summary>
    public async Task RemoveUserFromFarmAsync(string userId, string farmId)
    {
        // Validate parameters
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        
        if (string.IsNullOrEmpty(farmId))
            throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

        // Find the user profile
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId && p.FarmId == farmId);

        if (userProfile == null)
            throw new NotFoundException("User profile not found");

        // Remove the profile
        _context.UserProfiles.Remove(userProfile);
        await _context.SaveChangesAsync();

        // If this was the user's only profile with this role, and they have no other profiles with this role,
        // we may want to remove them from the role (optional, depending on business requirements)
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var profilesWithSameRole = await _context.UserProfiles
                .CountAsync(p => p.UserId == userId && p.Role == userProfile.Role);

            if (profilesWithSameRole == 0)
            {
                // Only remove from role if user has no other profiles with this role
                await _userManager.RemoveFromRoleAsync(user, userProfile.Role);
            }
        }
    }

    /// <summary>
    /// Gets all farm profiles for a user
    /// </summary>
    public async Task<IEnumerable<UserProfileViewModel>> GetUserFarmProfilesAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        var profiles = await _context.UserProfiles
            .Include(p => p.Farm)
            .Include(p => p.User)
            .Where(p => p.UserId == userId)
            .Select(p => new UserProfileViewModel
            {
                Id = p.Id,
                UserId = p.UserId,
                FarmId = p.FarmId,
                Farm = p.Farm.Name,
                Role = p.Role,
                Fullname = p.User.Fullname
            })
            .ToListAsync();

        return profiles;
    }

    /// <summary>
    /// Gets all users assigned to a farm
    /// </summary>
    public async Task<IEnumerable<UserProfileViewModel>> GetFarmUsersAsync(string farmId)
    {
        if (string.IsNullOrEmpty(farmId))
            throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

        var profiles = await _context.UserProfiles
            .Include(p => p.Farm)
            .Include(p => p.User)
            .Where(p => p.FarmId == farmId)
            .Select(p => new UserProfileViewModel
            {
                Id = p.Id,
                UserId = p.UserId,
                FarmId = p.FarmId,
                Farm = p.Farm.Name,
                Role = p.Role,
                Fullname = p.User.Fullname
            })
            .ToListAsync();

        return profiles;
    }

    /// <summary>
    /// Updates a user's role for a specific farm
    /// </summary>
    public async Task<UserProfileViewModel> UpdateUserFarmRoleAsync(string profileId, string newRole)
    {
        if (string.IsNullOrEmpty(profileId))
            throw new ArgumentException("Profile ID cannot be empty", nameof(profileId));
        
        if (string.IsNullOrEmpty(newRole))
            throw new ArgumentException("New role cannot be empty", nameof(newRole));

        // Check if role exists
        var roleExists = await _roleManager.RoleExistsAsync(newRole);
        if (!roleExists)
            throw new NotFoundException($"Role '{newRole}' not found");

        // Find the profile
        var profile = await _context.UserProfiles
            .Include(p => p.Farm)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == profileId);

        if (profile == null)
            throw new NotFoundException("User profile not found");

        // Check if the role is already assigned
        if (profile.Role == newRole)
            return new UserProfileViewModel
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FarmId = profile.FarmId,
                Farm = profile.Farm.Name,
                Role = profile.Role,
                Fullname = profile.User.Fullname
            };

        // Get old role for possible removal
        var oldRole = profile.Role;

        // Update the role
        profile.Role = newRole;
        _context.UserProfiles.Update(profile);
        await _context.SaveChangesAsync();

        // Update user's role assignment if needed
        var user = await _userManager.FindByIdAsync(profile.UserId);
        if (user != null)
        {
            // Assign new role
            await _userManager.AddToRoleAsync(user, newRole);

            // Check if we should remove old role
            var profilesWithOldRole = await _context.UserProfiles
                .CountAsync(p => p.UserId == profile.UserId && p.Role == oldRole);

            if (profilesWithOldRole == 0)
            {
                // Only remove from role if user has no other profiles with this role
                await _userManager.RemoveFromRoleAsync(user, oldRole);
            }
        }

        return new UserProfileViewModel
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FarmId = profile.FarmId,
            Farm = profile.Farm.Name,
            Role = newRole,
            Fullname = profile.User.Fullname
        };
    }
    
    /// <summary>
    /// Gets all farms that a user can access
    /// </summary>
    public async Task<IEnumerable<string>> GetUserAccessibleFarmIdsAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        // Check if user is a system administrator (can access all farms)
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var isSystemAdmin = await _userManager.IsInRoleAsync(user, "SystemAdministrator");
            if (isSystemAdmin)
            {
                // Return all farm IDs
                return await _context.Farms
                    .Where(f => f.IsActive && !f.Deleted)
                    .Select(f => f.Id)
                    .ToListAsync();
            }
        }

        // Otherwise, return only farms the user is assigned to
        return await _context.UserProfiles
            .Where(p => p.UserId == userId)
            .Select(p => p.FarmId)
            .ToListAsync();
    }
}