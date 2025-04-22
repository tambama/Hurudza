using System.Security.Claims;
using Hurudza.Data.Data;

namespace Hurudza.Common.Utils.Extensions
{
    /// <summary>
    /// Extension methods for checking permissions and roles in ClaimsPrincipal
    /// </summary>
    public static class PermissionExtensions
    {
        #region Permission Checking

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

        #endregion

        #region Feature-Specific Permission Checks

        /// <summary>
        /// Checks if the user can manage farms (create, edit, delete)
        /// </summary>
        public static bool CanManageFarms(this ClaimsPrincipal user)
        {
            return user.IsSystemAdmin() || 
                   user.HasPermission(Claims.FarmManage) || 
                  (user.HasPermission(Claims.FarmCreate) && user.HasPermission(Claims.FarmDelete));
        }

        /// <summary>
        /// Checks if the user can manage fields (create, edit, delete)
        /// </summary>
        public static bool CanManageFields(this ClaimsPrincipal user)
        {
            return user.IsSystemAdmin() || 
                   user.HasPermission(Claims.FieldManage) || 
                  (user.HasPermission(Claims.FieldCreate) && user.HasPermission(Claims.FieldDelete));
        }

        /// <summary>
        /// Checks if the user can manage crops (create, edit, delete)
        /// </summary>
        public static bool CanManageCrops(this ClaimsPrincipal user)
        {
            return user.IsSystemAdmin() || 
                   user.HasPermission(Claims.CropManage) || 
                  (user.HasPermission(Claims.CropCreate) && user.HasPermission(Claims.CropDelete));
        }

        /// <summary>
        /// Checks if the user can manage users (create, edit, delete)
        /// </summary>
        public static bool CanManageUsers(this ClaimsPrincipal user)
        {
            return user.IsSystemAdmin() || 
                   user.HasPermission(Claims.UserManage) || 
                  (user.HasPermission(Claims.UserCreate) && user.HasPermission(Claims.UserDelete));
        }

        /// <summary>
        /// Checks if the user can manage roles (create, edit, delete)
        /// </summary>
        public static bool CanManageRoles(this ClaimsPrincipal user)
        {
            return user.IsSystemAdmin() || 
                   user.HasPermission(Claims.RoleManage) || 
                  (user.HasPermission(Claims.RoleCreate) && user.HasPermission(Claims.RoleDelete));
        }

        /// <summary>
        /// Checks if the user can manage tillage operations (create, edit, delete)
        /// </summary>
        public static bool CanManageTillage(this ClaimsPrincipal user)
        {
            return user.IsSystemAdmin() || 
                   user.HasPermission(Claims.TillageManage) || 
                  (user.HasPermission(Claims.TillageCreate) && user.HasPermission(Claims.TillageDelete));
        }

        #endregion

        #region Claim and Role Helpers

        /// <summary>
        /// Gets the current farm ID from claims
        /// </summary>
        public static string GetCurrentFarmId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("FarmId") ?? string.Empty;
        }

        /// <summary>
        /// Gets the current farm name from claims
        /// </summary>
        public static string GetCurrentFarmName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("FarmName") ?? string.Empty;
        }

        /// <summary>
        /// Gets the user's primary role
        /// </summary>
        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }

        /// <summary>
        /// Gets the user's ID from available claim types
        /// </summary>
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                   user.FindFirstValue(ClaimTypes.PrimarySid) ?? 
                   user.FindFirstValue("UserId") ?? 
                   string.Empty;
        }

        #endregion

        #region Role-Based Checks

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
            return user.IsInRole(ApiRoles.SystemAdministrator) || 
                   user.IsInRole(ApiRoles.Administrator) ||
                   user.IsInRole(ApiRoles.FarmAdministrator);
        }

        /// <summary>
        /// Check if user is a farm manager
        /// </summary>
        public static bool IsFarmManager(this ClaimsPrincipal user)
        {
            return user.IsSystemAdmin() || 
                   user.IsAdministrator() ||
                   user.IsInRole(ApiRoles.FarmManager);
        }

        /// <summary>
        /// Check if user is a field officer
        /// </summary>
        public static bool IsFieldOfficer(this ClaimsPrincipal user)
        {
            return user.IsSystemAdmin() || 
                   user.IsAdministrator() ||
                   user.IsFarmManager() ||
                   user.IsInRole(ApiRoles.FieldOfficer);
        }

        /// <summary>
        /// Check if user has at least viewer access
        /// </summary>
        public static bool IsViewer(this ClaimsPrincipal user)
        {
            // All authenticated users should have at least viewer permissions
            return user.Identity?.IsAuthenticated ?? false;
        }

        #endregion
    }
}