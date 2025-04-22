using Hurudza.Common.Utils.Exceptions;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Data;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hurudza.Data.Services.Services
{
    /// <summary>
    /// Enhanced service for managing users' farm assignments and roles
    /// </summary>
    public class FarmUserManagerService : IFarmUserManagerService
    {
        private readonly HurudzaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<FarmUserManagerService> _logger;

        public FarmUserManagerService(
            HurudzaDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<FarmUserManagerService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Gets all users assigned to a farm with their roles and permissions
        /// </summary>
        public async Task<List<FarmUserViewModel>> GetFarmUsersAsync(string farmId)
        {
            if (string.IsNullOrEmpty(farmId))
                throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

            // Check if farm exists
            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.Id == farmId && f.IsActive && !f.Deleted);
                
            if (farm == null)
                throw new NotFoundException($"Farm with ID '{farmId}' not found");

            var profiles = await _context.UserProfiles
                .Include(p => p.User)
                .Where(p => p.FarmId == farmId)
                .ToListAsync();

            var farmUsers = new List<FarmUserViewModel>();

            foreach (var profile in profiles)
            {
                if (profile.User == null) // Skip if user association is missing
                    continue;

                var user = profile.User;
                
                // Get user roles and permissions
                var roles = await _userManager.GetRolesAsync(user);
                var permissions = new List<string>();
                
                // Get permissions from roles
                foreach (var roleName in roles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var roleClaims = await _roleManager.GetClaimsAsync(role);
                        permissions.AddRange(roleClaims
                            .Where(c => c.Type == "Permission")
                            .Select(c => c.Value));
                    }
                }
                
                // Get direct user permissions
                var userClaims = await _userManager.GetClaimsAsync(user);
                permissions.AddRange(userClaims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value));
                
                // Remove duplicates
                permissions = permissions.Distinct().ToList();

                farmUsers.Add(new FarmUserViewModel
                {
                    ProfileId = profile.Id,
                    UserId = user.Id,
                    Username = user.UserName,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FarmId = profile.FarmId,
                    FarmName = farm.Name,
                    Role = profile.Role,
                    AssignedDate = profile.CreatedDate,
                    IsActive = profile.IsActive && user.IsActive,
                    Permissions = permissions
                });
            }

            return farmUsers;
        }

        /// <summary>
        /// Gets a summary of users for a farm
        /// </summary>
        public async Task<FarmUserSummaryViewModel> GetFarmUserSummaryAsync(string farmId)
        {
            if (string.IsNullOrEmpty(farmId))
                throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

            // Check if farm exists
            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.Id == farmId && f.IsActive && !f.Deleted);
                
            if (farm == null)
                throw new NotFoundException($"Farm with ID '{farmId}' not found");

            var profiles = await _context.UserProfiles
                .Where(p => p.FarmId == farmId && p.IsActive)
                .ToListAsync();

            return new FarmUserSummaryViewModel
            {
                FarmId = farmId,
                FarmName = farm.Name,
                TotalUsers = profiles.Count,
                Administrators = profiles.Count(p => p.Role == ApiRoles.Administrator || p.Role == ApiRoles.FarmAdministrator),
                Managers = profiles.Count(p => p.Role == ApiRoles.FarmManager),
                FieldOfficers = profiles.Count(p => p.Role == ApiRoles.FieldOfficer),
                Viewers = profiles.Count(p => p.Role == ApiRoles.Viewer),
                LastUpdated = DateTime.Now
            };
        }

        /// <summary>
        /// Gets all farms a user has access to
        /// </summary>
        public async Task<List<UserFarmProfileViewModel>> GetUserFarmProfilesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID '{userId}' not found");

            var profiles = await _context.UserProfiles
                .Include(p => p.Farm)
                .Where(p => p.UserId == userId && p.Farm.IsActive && !p.Farm.Deleted)
                .ToListAsync();

            var userFarmProfiles = profiles.Select(p => new UserFarmProfileViewModel
            {
                ProfileId = p.Id,
                FarmId = p.FarmId,
                FarmName = p.Farm?.Name ?? "Unknown Farm",
                Role = p.Role,
                AssignedDate = p.CreatedDate,
                IsActive = p.IsActive
            }).ToList();

            return userFarmProfiles;
        }

        /// <summary>
        /// Gets detailed information about a user's access across farms
        /// </summary>
        public async Task<UserFarmAccessViewModel> GetUserFarmAccessAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID '{userId}' not found");

            var roles = await _userManager.GetRolesAsync(user);
            var isSystemAdmin = roles.Contains(ApiRoles.SystemAdministrator);

            var profiles = await _context.UserProfiles
                .Include(p => p.Farm)
                .Where(p => p.UserId == userId && p.Farm.IsActive && !p.Farm.Deleted)
                .ToListAsync();

            var farmProfiles = profiles.Select(p => new UserFarmProfileViewModel
            {
                ProfileId = p.Id,
                FarmId = p.FarmId,
                FarmName = p.Farm?.Name ?? "Unknown Farm",
                Role = p.Role,
                AssignedDate = p.CreatedDate,
                IsActive = p.IsActive
            }).ToList();

            return new UserFarmAccessViewModel
            {
                UserId = userId,
                Username = user.UserName,
                Fullname = user.Fullname,
                FarmProfiles = farmProfiles,
                IsSystemAdministrator = isSystemAdmin,
                LastLogin = user.CreatedDate // Replace with actual last login when available
            };
        }

        /// <summary>
        /// Assigns a user to a farm with the specified role
        /// </summary>
        public async Task<UserProfileViewModel> AssignUserToFarmAsync(string userId, string farmId, string role)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
            if (string.IsNullOrEmpty(farmId))
                throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));
            
            if (string.IsNullOrEmpty(role))
                throw new ArgumentException("Role cannot be empty", nameof(role));

            // Check if user exists
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID '{userId}' not found");

            // Check if farm exists
            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.Id == farmId && f.IsActive && !f.Deleted);
                
            if (farm == null)
                throw new NotFoundException($"Farm with ID '{farmId}' not found");

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
                    existingProfile.IsActive = true; // Ensure it's active
                    _context.UserProfiles.Update(existingProfile);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation($"Updated role for user {userId} on farm {farmId} to {role}");
                }
                else if (!existingProfile.IsActive)
                {
                    // Reactivate if inactive
                    existingProfile.IsActive = true;
                    _context.UserProfiles.Update(existingProfile);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation($"Reactivated profile for user {userId} on farm {farmId}");
                }

                // Ensure the user is assigned to the role in ASP.NET Identity
                await EnsureUserInRoleAsync(user, role);

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
                Role = role,
                IsActive = true
            };

            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Created new profile for user {userId} on farm {farmId} with role {role}");

            // Ensure the user is assigned to the role
            await EnsureUserInRoleAsync(user, role);

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
        /// Updates a user's role for a specific farm
        /// </summary>
        public async Task<UserProfileViewModel> UpdateUserFarmRoleAsync(string profileId, string newRole, bool isActive = true)
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
                throw new NotFoundException($"User profile with ID '{profileId}' not found");

            // Get old role for possible removal
            var oldRole = profile.Role;
            
            // Update the role and active status
            profile.Role = newRole;
            profile.IsActive = isActive;
            
            _context.UserProfiles.Update(profile);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Updated profile {profileId} for user {profile.UserId} from role {oldRole} to {newRole}, active: {isActive}");

            // Update user's role assignment if needed
            if (profile.User != null)
            {
                // Assign new role
                await EnsureUserInRoleAsync(profile.User, newRole);

                // Check if we should remove old role
                if (oldRole != newRole)
                {
                    await CleanupUnusedRolesAsync(profile.User, oldRole);
                }
            }

            return new UserProfileViewModel
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FarmId = profile.FarmId,
                Farm = profile.Farm?.Name ?? "Unknown Farm",
                Role = newRole,
                Fullname = profile.User?.Fullname ?? "Unknown User"
            };
        }

        /// <summary>
        /// Updates a user's active status for a specific farm
        /// </summary>
        public async Task<UserProfileViewModel> UpdateUserFarmStatusAsync(string profileId, bool isActive)
        {
            if (string.IsNullOrEmpty(profileId))
                throw new ArgumentException("Profile ID cannot be empty", nameof(profileId));

            // Find the profile
            var profile = await _context.UserProfiles
                .Include(p => p.Farm)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == profileId);

            if (profile == null)
                throw new NotFoundException($"User profile with ID '{profileId}' not found");

            // Update the active status
            profile.IsActive = isActive;
            
            _context.UserProfiles.Update(profile);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Updated profile {profileId} active status to {isActive}");

            return new UserProfileViewModel
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FarmId = profile.FarmId,
                Farm = profile.Farm?.Name ?? "Unknown Farm",
                Role = profile.Role,
                Fullname = profile.User?.Fullname ?? "Unknown User"
            };
        }

        /// <summary>
        /// Removes a user's assignment from a farm
        /// </summary>
        public async Task RemoveUserFromFarmAsync(string userId, string farmId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
            if (string.IsNullOrEmpty(farmId))
                throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

            // Find the user profile
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId && p.FarmId == farmId);

            if (userProfile == null)
                throw new NotFoundException($"Profile for user '{userId}' on farm '{farmId}' not found");

            // Store role for cleanup
            var role = userProfile.Role;

            // Remove the profile
            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Removed profile for user {userId} from farm {farmId}");

            // Check if user has other profiles with this role, and remove role if not
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await CleanupUnusedRolesAsync(user, role);
            }
        }

        /// <summary>
        /// Gets all farm IDs that a user has access to
        /// </summary>
        public async Task<List<string>> GetUserAccessibleFarmIdsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            // Check if user exists
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID '{userId}' not found");

            // Check if user is a system administrator (can access all farms)
            if (await _userManager.IsInRoleAsync(user, ApiRoles.SystemAdministrator))
            {
                // Return all farm IDs
                return await _context.Farms
                    .Where(f => f.IsActive && !f.Deleted)
                    .Select(f => f.Id)
                    .ToListAsync();
            }

            // Otherwise, return only farms the user is assigned to
            return await _context.UserProfiles
                .Where(p => p.UserId == userId && p.IsActive)
                .Select(p => p.FarmId)
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets statistics about user assignments across all farms
        /// </summary>
        public async Task<List<FarmUserSummaryViewModel>> GetSystemUserAssignmentStatisticsAsync()
        {
            var farmSummaries = new List<FarmUserSummaryViewModel>();
            
            var farms = await _context.Farms
                .Where(f => f.IsActive && !f.Deleted)
                .ToListAsync();
                
            foreach (var farm in farms)
            {
                var profiles = await _context.UserProfiles
                    .Where(p => p.FarmId == farm.Id && p.IsActive)
                    .ToListAsync();
                    
                farmSummaries.Add(new FarmUserSummaryViewModel
                {
                    FarmId = farm.Id,
                    FarmName = farm.Name,
                    TotalUsers = profiles.Count,
                    Administrators = profiles.Count(p => p.Role == ApiRoles.Administrator || p.Role == ApiRoles.FarmAdministrator),
                    Managers = profiles.Count(p => p.Role == ApiRoles.FarmManager),
                    FieldOfficers = profiles.Count(p => p.Role == ApiRoles.FieldOfficer),
                    Viewers = profiles.Count(p => p.Role == ApiRoles.Viewer),
                    LastUpdated = DateTime.Now
                });
            }
            
            return farmSummaries;
        }

        #region Helper Methods

        /// <summary>
        /// Ensures a user is in the specified role
        /// </summary>
        private async Task EnsureUserInRoleAsync(ApplicationUser user, string role)
        {
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
                _logger.LogInformation($"Added user {user.Id} to role {role}");
            }
        }

        /// <summary>
        /// Remove user from a role if they don't have any more farm profiles with that role
        /// </summary>
        private async Task CleanupUnusedRolesAsync(ApplicationUser user, string roleToCheck)
        {
            // Skip cleanup for system administrator
            if (roleToCheck == ApiRoles.SystemAdministrator)
                return;
                
            var profilesWithSameRole = await _context.UserProfiles
                .CountAsync(p => p.UserId == user.Id && p.Role == roleToCheck && p.IsActive);

            if (profilesWithSameRole == 0 && await _userManager.IsInRoleAsync(user, roleToCheck))
            {
                await _userManager.RemoveFromRoleAsync(user, roleToCheck);
                _logger.LogInformation($"Removed user {user.Id} from role {roleToCheck} as they have no active profiles with this role");
            }
        }

        #endregion
    }
}