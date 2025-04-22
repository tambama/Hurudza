using Hurudza.Data.Context.Context;
using Hurudza.Data.Data;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hurudza.Data.Services
{
    /// <summary>
    /// Service for initializing roles and permissions in the system
    /// </summary>
    public class RoleInitializerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RoleInitializerService> _logger;
        
        public RoleInitializerService(
            IServiceProvider serviceProvider,
            ILogger<RoleInitializerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        
        /// <summary>
        /// Initialize all roles and permissions in the system
        /// </summary>
        public async Task InitializeRolesAndPermissions()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var context = scope.ServiceProvider.GetRequiredService<HurudzaDbContext>();
                
                // Create all permissions
                await InitializePermissions(context);
                
                // Create all roles
                await InitializeRoles(roleManager);
                
                // Assign default permissions to roles
                await AssignDefaultPermissionsToRoles(roleManager);
                
                _logger.LogInformation("Roles and permissions initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing roles and permissions");
                throw;
            }
        }
        
        /// <summary>
        /// Initialize all permissions in the system
        /// </summary>
        private async Task InitializePermissions(HurudzaDbContext context)
        {
            _logger.LogInformation("Initializing permissions...");
            
            var allPermissions = Claims.GetAllPermissions();
            var existingClaims = await context.Claims.ToListAsync();
            int addedCount = 0;
            
            // Flatten the grouped permissions and create any missing ones
            foreach (var group in allPermissions)
            {
                foreach (var permission in group.Value)
                {
                    if (!existingClaims.Any(c => c.ClaimType == "Permission" && c.ClaimValue == permission))
                    {
                        await context.Claims.AddAsync(new IdentityClaim
                        {
                            ClaimType = "Permission",
                            ClaimValue = permission
                        });
                        addedCount++;
                    }
                }
            }
            
            if (addedCount > 0)
            {
                await context.SaveChangesAsync();
                _logger.LogInformation($"Added {addedCount} new permission claims");
            }
            else
            {
                _logger.LogInformation("All permissions already exist");
            }
        }
        
        /// <summary>
        /// Initialize all roles in the system
        /// </summary>
        private async Task InitializeRoles(RoleManager<ApplicationRole> roleManager)
        {
            _logger.LogInformation("Initializing roles...");
            
            var allRoles = ApiRoles.GetAllRoles();
            var addedCount = 0;
            
            // Organize roles by their RoleClass and create any missing ones
            foreach (var categoryGroup in allRoles)
            {
                var roleClass = Enum.Parse<Hurudza.Data.Enums.Enums.RoleClass>(categoryGroup.Key);
                
                foreach (var roleName in categoryGroup.Value)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        // Create description based on role name
                        string description = FormatRoleDescription(roleName);
                        
                        var result = await roleManager.CreateAsync(new ApplicationRole
                        {
                            Name = roleName,
                            Description = description,
                            RoleClass = roleClass
                        });
                        
                        if (result.Succeeded)
                        {
                            addedCount++;
                        }
                        else
                        {
                            _logger.LogError($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }
            }
            
            _logger.LogInformation($"Added {addedCount} new roles");
        }
        
        /// <summary>
        /// Assign default permissions to roles
        /// </summary>
        private async Task AssignDefaultPermissionsToRoles(RoleManager<ApplicationRole> roleManager)
        {
            _logger.LogInformation("Assigning default permissions to roles...");
            
            var defaultPermissions = ApiRoles.GetDefaultRolePermissions();
            int assignedCount = 0;
            
            foreach (var roleName in defaultPermissions.Keys)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning($"Role {roleName} not found when assigning permissions");
                    continue;
                }
                
                // Get existing claims for this role
                var existingClaims = await roleManager.GetClaimsAsync(role);
                
                // Add missing claims
                foreach (var permission in defaultPermissions[roleName])
                {
                    if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                    {
                        var result = await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                        if (result.Succeeded)
                        {
                            assignedCount++;
                        }
                        else
                        {
                            _logger.LogError($"Failed to assign permission {permission} to role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }
            }
            
            _logger.LogInformation($"Assigned {assignedCount} permissions to roles");
        }
        
        /// <summary>
        /// Format a role name into a human-readable description
        /// </summary>
        private string FormatRoleDescription(string roleName)
        {
            return roleName switch
            {
                ApiRoles.SystemAdministrator => "System Administrator with full access",
                ApiRoles.Administrator => "Administrator with access to manage farms and users",
                ApiRoles.FarmManager => "Farm Manager with access to manage farm operations",
                ApiRoles.FarmAdministrator => "Farm Administrator with access to manage farm and users",
                ApiRoles.FieldOfficer => "Field Officer with access to manage crops and fields",
                ApiRoles.FarmOperator => "Farm Operator with limited operation access",
                ApiRoles.Viewer => "Viewer with read-only access",
                _ => roleName // Use role name if no specific description
            };
        }
    }
}