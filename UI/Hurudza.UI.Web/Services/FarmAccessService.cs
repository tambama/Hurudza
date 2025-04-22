using System.Security.Claims;
using Hurudza.Data.Data;
using Hurudza.UI.Web.Api.Interfaces;
using Hurudza.UI.Web.Cookie.Providers;
using Hurudza.UI.Web.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Hurudza.UI.Web.Services;

public class FarmAccessService
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly IApiCall _apiCall;

    public FarmAccessService(AuthenticationStateProvider authProvider, IApiCall apiCall)
    {
        _authProvider = authProvider;
        _apiCall = apiCall;
    }

    /// <summary>
    /// Checks if the current user can access a specific farm
    /// </summary>
    public async Task<bool> CanAccessFarm(string farmId)
    {
        if (string.IsNullOrEmpty(farmId))
            return false;

        // Get the authentication state
        var authState = await _authProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        // System administrators can access all farms
        if (user.IsInRole(ApiRoles.SystemAdministrator))
            return true;

        // Check if this is the user's current farm
        var currentFarmId = user.FindFirstValue("FarmId");
        if (currentFarmId == farmId)
            return true;

        // Otherwise, check if the user has any profile for this farm
        // This requires an API call since we don't store all profiles in claims
        try
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                         user.FindFirstValue("UserId");
                         
            if (string.IsNullOrEmpty(userId))
                return false;

            var accessibleFarms = await GetUserAccessibleFarms(userId);
            return accessibleFarms.Contains(farmId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking farm access: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets all farm IDs the user can access
    /// </summary>
    public async Task<List<string>> GetUserAccessibleFarms(string userId)
    {
        try
        {
            var response = await _apiCall.Get<ApiResponse<List<string>>>(
                await _apiCall.GetHttpClient(), $"farmusers/getaccessiblefarms?userId={userId}");

            if (response?.Status == 200 && response.Result != null)
            {
                return response.Result;
            }
            
            return new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting accessible farms: {ex.Message}");
            return new List<string>();
        }
    }

    /// <summary>
    /// Checks if current user can manage a specific farm (admin or farm manager)
    /// </summary>
    public async Task<bool> CanManageFarm(string farmId)
    {
        var authState = await _authProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        // System administrators can manage all farms
        if (user.IsInRole(ApiRoles.SystemAdministrator))
            return true;

        // For other roles, they need to be assigned to this specific farm
        // and have a management role
        var currentFarmId = user.FindFirstValue("FarmId");
        if (currentFarmId != farmId)
            return false;

        // Check if user is an admin or farm manager
        return user.IsInRole(ApiRoles.Administrator) || user.IsInRole("FarmManager");
    }

    /// <summary>
    /// Checks if the user has a specific permission for the current farm
    /// </summary>
    public async Task<bool> HasPermission(string permission)
    {
        var customProvider = _authProvider as CustomAuthStateProvider;
        if (customProvider != null)
        {
            return customProvider.UserHasPermission(permission);
        }

        // Fallback to standard claim check
        var authState = await _authProvider.GetAuthenticationStateAsync();
        return authState.User.HasClaim(c => c.Type == "Permission" && c.Value == permission);
    }

    /// <summary>
    /// Checks if the user has any of the specified permissions for the current farm
    /// </summary>
    public async Task<bool> HasAnyPermission(params string[] permissions)
    {
        var customProvider = _authProvider as CustomAuthStateProvider;
        if (customProvider != null)
        {
            return customProvider.UserHasAnyPermission(permissions);
        }

        // Fallback to standard claim check
        var authState = await _authProvider.GetAuthenticationStateAsync();
        return authState.User.Claims
            .Where(c => c.Type == "Permission")
            .Any(c => permissions.Contains(c.Value));
    }

    /// <summary>
    /// Gets the current farm ID from claims
    /// </summary>
    public async Task<string> GetCurrentFarmId()
    {
        var customProvider = _authProvider as CustomAuthStateProvider;
        if (customProvider != null)
        {
            return customProvider.GetCurrentFarmId();
        }

        var authState = await _authProvider.GetAuthenticationStateAsync();
        return authState.User.FindFirstValue("FarmId") ?? string.Empty;
    }

    /// <summary>
    /// Gets the current farm name from claims
    /// </summary>
    public async Task<string> GetCurrentFarmName()
    {
        var customProvider = _authProvider as CustomAuthStateProvider;
        if (customProvider != null)
        {
            return customProvider.GetCurrentFarmName();
        }

        var authState = await _authProvider.GetAuthenticationStateAsync();
        return authState.User.FindFirstValue("FarmName") ?? string.Empty;
    }
}