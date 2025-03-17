using System.Security.Claims;
using Hurudza.Data.Data;
using Hurudza.UI.Web.Cookie.Providers;
using Microsoft.AspNetCore.Components.Authorization;

namespace Hurudza.UI.Web.Helpers;

public static class PermissionHelper
{
    public static bool HasPermission(this ClaimsPrincipal user, string permission)
    {
        return user.HasClaim(c => c.Type == "Permission" && c.Value == permission);
    }
    
    public static bool HasAnyPermission(this ClaimsPrincipal user, params string[] permissions)
    {
        return user.Claims
            .Where(c => c.Type == "Permission")
            .Any(c => permissions.Contains(c.Value));
    }
    
    public static bool HasAllPermissions(this ClaimsPrincipal user, params string[] permissions)
    {
        var userPermissions = user.Claims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();
        
        return permissions.All(p => userPermissions.Contains(p));
    }
    
    public static string GetCurrentFarmId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("FarmId") ?? string.Empty;
    }
    
    public static string GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
    
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("UserId") ?? string.Empty;
    }
    
    public static bool IsSystemAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole(ApiRoles.SystemAdministrator);
    }
    
    public static bool IsAdministrator(this ClaimsPrincipal user)
    {
        return user.IsInRole(ApiRoles.Administrator) || user.IsInRole(ApiRoles.SystemAdministrator);
    }
    
    public static bool IsFarmManager(this ClaimsPrincipal user)
    {
        return user.IsInRole(ApiRoles.FarmManager);
    }
    
    public static async Task<bool> CanManageFarms(this AuthenticationStateProvider provider)
    {
        var authState = await provider.GetAuthenticationStateAsync();
        return authState.User.HasPermission(Claims.FarmManage);
    }
    
    public static async Task<bool> CanManageUsers(this AuthenticationStateProvider provider)
    {
        var authState = await provider.GetAuthenticationStateAsync();
        return authState.User.HasPermission(Claims.UserManage);
    }
    
    public static async Task<bool> CanManageFields(this AuthenticationStateProvider provider)
    {
        var authState = await provider.GetAuthenticationStateAsync();
        return authState.User.HasPermission(Claims.FieldManage);
    }
}