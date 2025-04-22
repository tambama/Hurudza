using System.Security.Claims;
using Hurudza.Data.Data;

namespace Hurudza.Common.Utils.Extensions
{
    /// <summary>
    /// Extension methods for working with user permissions
    /// </summary>
    public static class PermissionExtensions
    {
        /// <summary>
        /// Checks if the user has a specific permission
        /// </summary>
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            return user.HasClaim(c => c.Type == "Permission" && c.Value == permission);
        }

        /// <summary>
        /// Checks if the user has any of the specified permissions
        /// </summary>
        public static bool HasAnyPermission(this ClaimsPrincipal user, params string[] permissions)
        {
            return user.Claims
                .Where(c => c.Type == "Permission")
                .Any(c => permissions.Contains(c.Value));
        }

        /// <summary>
        /// Checks if the user has all of the specified permissions
        /// </summary>
        public static bool HasAllPermissions(this ClaimsPrincipal user, params string[] permissions)
        {
            var userPermissions = user.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            return permissions.All(p => userPermissions.Contains(p));
        }

        /// <summary>
        /// Checks if the user can manage farms (create, edit, delete)
        /// </summary>
        public static bool CanManageFarms(this ClaimsPrincipal user)
        {
            return user.HasPermission(Claims.FarmManage) || 
                  (user.HasPermission(Claims.FarmCreate) && user.HasPermission(Claims.FarmDelete));
        }

        /// <summary>
        /// Checks if the user can manage fields (create, edit, delete)
        /// </summary>
        public static bool CanManageFields(this ClaimsPrincipal user)
        {
            return user.HasPermission(Claims.FieldManage) || 
                  (user.HasPermission(Claims.FieldCreate) && user.HasPermission(Claims.FieldDelete));
        }

        /// <summary>
        /// Checks if the user can manage crops (create, edit, delete)
        /// </summary>
        public static bool CanManageCrops(this ClaimsPrincipal user)
        {
            return user.HasPermission(Claims.CropManage) || 
                  (user.HasPermission(Claims.CropCreate) && user.HasPermission(Claims.CropDelete));
        }

        /// <summary>
        /// Checks if the user can manage users (create, edit, delete)
        /// </summary>
        public static bool CanManageUsers(this ClaimsPrincipal user)
        {
            return user.HasPermission(Claims.UserManage) || 
                  (user.HasPermission(Claims.UserCreate) && user.HasPermission(Claims.UserDelete));
        }

        /// <summary>
        /// Checks if the user can manage roles (create, edit, delete)
        /// </summary>
        public static bool CanManageRoles(this ClaimsPrincipal user)
        {
            return user.HasPermission(Claims.RoleManage) || 
                  (user.HasPermission(Claims.RoleCreate) && user.HasPermission(Claims.RoleDelete));
        }

        /// <summary>
        /// Gets the current farm ID from claims
        /// </summary>
        public static string GetCurrentFarmId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("FarmId") ?? string.Empty;
        }

        /// <summary>
        /// Gets the user's primary role
        /// </summary>
        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }

        /// <summary>
        /// Gets the user's ID
        /// </summary>
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                   user.FindFirstValue(ClaimTypes.PrimarySid) ?? 
                   user.FindFirstValue("UserId") ?? 
                   string.Empty;
        }

        /// <summary>
        /// Check if user is a system administrator
        /// </summary>
        public static bool IsSystemAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole(ApiRoles.SystemAdministrator);
        }

        /// <summary>
        /// Check if user is an administrator (system or regular)
        /// </summary>
        public static bool IsAdministrator(this ClaimsPrincipal user)
        {
            return user.IsInRole(ApiRoles.Administrator) || 
                   user.IsInRole(ApiRoles.SystemAdministrator);
        }

        /// <summary>
        /// Check if user is a farm manager
        /// </summary>
        public static bool IsFarmManager(this ClaimsPrincipal user)
        {
            return user.IsInRole(ApiRoles.FarmManager);
        }
    }
}