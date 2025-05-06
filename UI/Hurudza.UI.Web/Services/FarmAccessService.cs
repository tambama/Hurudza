using System.Security.Claims;
using Hurudza.Common.Utils.Extensions;
using Hurudza.Data.Data;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Hurudza.UI.Web.Api.Interfaces;
using Hurudza.UI.Web.Cookie.Providers;
using Hurudza.UI.Web.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Hurudza.UI.Web.Services;

/// <summary>
/// Service for handling farm access permissions and related operations
/// </summary>
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
        if (user.IsSystemAdmin())
            return true;

        // Check if this is the user's current farm
        var currentFarmId = user.GetCurrentFarmId();
        if (currentFarmId == farmId)
            return true;

        // Otherwise, check if the user has any profile for this farm
        try
        {
            var userId = user.GetUserId();
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
                await _apiCall.GetHttpClient(), $"farmusers/getaccessiblefarms/{userId}");

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
        if (user.IsSystemAdmin())
            return true;

        // For other roles, they need to be assigned to this specific farm and have a management role
        if (!await CanAccessFarm(farmId))
            return false;

        var currentFarmId = user.GetCurrentFarmId();
        if (currentFarmId != farmId)
            return false;

        // Check if user is an admin or farm manager for this farm
        return user.IsAdministrator() || user.IsFarmManager();
    }

    /// <summary>
    /// Checks if the current user has a specific permission for the current farm
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
        return authState.User.HasPermission(permission);
    }

    /// <summary>
    /// Checks if the current user has any of the specified permissions
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
        return authState.User.HasAnyPermission(permissions);
    }

    /// <summary>
    /// Checks if the current user has all of the specified permissions
    /// </summary>
    public async Task<bool> HasAllPermissions(params string[] permissions)
    {
        var customProvider = _authProvider as CustomAuthStateProvider;
        if (customProvider != null)
        {
            return customProvider.UserHasAllPermissions(permissions);
        }

        // Fallback to standard claim check
        var authState = await _authProvider.GetAuthenticationStateAsync();
        return authState.User.HasAllPermissions(permissions);
    }

    /// <summary>
    /// Checks if the current user can manage fields on the given farm
    /// </summary>
    public async Task<bool> CanManageFields(string farmId)
    {
        if (string.IsNullOrEmpty(farmId))
            return false;

        // First check if user can access the farm
        if (!await CanAccessFarm(farmId))
            return false;

        var authState = await _authProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        // Check field management permission
        if (user.CanManageFields())
            return true;

        // Also check role-based access
        return user.IsSystemAdmin() || user.IsAdministrator() || user.IsFarmManager();
    }

    /// <summary>
    /// Checks if the current user can manage crops on the given farm
    /// </summary>
    public async Task<bool> CanManageCrops(string farmId)
    {
        if (string.IsNullOrEmpty(farmId))
            return false;

        // First check if user can access the farm
        if (!await CanAccessFarm(farmId))
            return false;

        var authState = await _authProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        // Check crop management permission
        if (user.CanManageCrops())
            return true;

        // Also check role-based access
        return user.IsSystemAdmin() || user.IsAdministrator() || user.IsFarmManager() || user.IsFieldOfficer();
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
        return authState.User.GetCurrentFarmId();
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
        return authState.User.GetCurrentFarmName();
    }

    /// <summary>
    /// Gets the user's current role
    /// </summary>
    public async Task<string> GetCurrentRole()
    {
        var authState = await _authProvider.GetAuthenticationStateAsync();
        return authState.User.GetUserRole();
    }

    /// <summary>
    /// Gets all farms the current user can access
    /// </summary>
    public async Task<List<FarmListViewModel>> GetAccessibleFarms()
    {
        try
        {
            var userId = "";
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                userId = user.GetUserId();
            }

            var response = await _apiCall.Get<ApiResponse<List<FarmListViewModel>>>(
                await _apiCall.GetHttpClient(), $"farmusers/getaccessiblefarms{(string.IsNullOrEmpty(userId) ? "" : $"/{userId}")}");

            if (response?.Status == 200 && response.Result != null)
            {
                return response.Result;
            }

            return new List<FarmListViewModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting accessible farms: {ex.Message}");
            return new List<FarmListViewModel>();
        }
    }
}