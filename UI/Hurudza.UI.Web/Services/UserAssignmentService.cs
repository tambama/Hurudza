using Hurudza.Data.UI.Models.ViewModels.Core;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Hurudza.UI.Web.Api.Interfaces;
using Hurudza.UI.Web.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Security.Claims;

namespace Hurudza.UI.Web.Services
{
    /// <summary>
    /// Service to handle farm user assignments from the UI
    /// </summary>
    public class UserAssignmentService
    {
        private readonly IApiCall _apiCall;
        private readonly AuthenticationStateProvider _authProvider;
        private readonly FarmAccessService _farmAccessService;

        public UserAssignmentService(
            IApiCall apiCall,
            AuthenticationStateProvider authProvider,
            FarmAccessService farmAccessService)
        {
            _apiCall = apiCall;
            _authProvider = authProvider;
            _farmAccessService = farmAccessService;
        }

        /// <summary>
        /// Gets all users assigned to a specific farm
        /// </summary>
        public async Task<List<FarmUserViewModel>> GetFarmUsersAsync(string farmId)
        {
            try
            {
                // Check if user has access to this farm
                bool canAccess = await _farmAccessService.CanAccessFarm(farmId);
                if (!canAccess)
                {
                    return new List<FarmUserViewModel>();
                }

                var response = await _apiCall.Get<ApiResponse<List<FarmUserViewModel>>>(
                    await _apiCall.GetHttpClient(), $"farmusers/getfarmusers?farmId={farmId}");

                if (response?.Status == (int)HttpStatusCode.OK && response.Result != null)
                {
                    return response.Result;
                }

                return new List<FarmUserViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting farm users: {ex.Message}");
                return new List<FarmUserViewModel>();
            }
        }

        /// <summary>
        /// Gets users not currently assigned to the specified farm
        /// </summary>
        public async Task<List<UserViewModel>> GetAvailableUsersForFarmAsync(string farmId)
        {
            try
            {
                // Get all users
                var allUsersResponse = await _apiCall.Get<ApiResponse<List<UserViewModel>>>(
                    await _apiCall.GetHttpClient(), "users/getusers");

                if (allUsersResponse?.Status != (int)HttpStatusCode.OK || allUsersResponse.Result == null)
                {
                    return new List<UserViewModel>();
                }

                // Get users already on this farm
                var farmUsers = await GetFarmUsersAsync(farmId);
                var existingUserIds = farmUsers.Select(u => u.UserId).ToList();

                // Filter out users already assigned to this farm
                return allUsersResponse.Result
                    .Where(u => !existingUserIds.Contains(u.Id))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting available users: {ex.Message}");
                return new List<UserViewModel>();
            }
        }

        /// <summary>
        /// Gets all available farm roles for assignment
        /// </summary>
        public async Task<List<RoleViewModel>> GetAvailableFarmRolesAsync()
        {
            try
            {
                var response = await _apiCall.Get<ApiResponse<List<RoleViewModel>>>(
                    await _apiCall.GetHttpClient(), "farmusers/getavailablefarmroles");

                if (response?.Status == (int)HttpStatusCode.OK && response.Result != null)
                {
                    return response.Result;
                }

                return new List<RoleViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting available roles: {ex.Message}");
                return new List<RoleViewModel>();
            }
        }

        /// <summary>
        /// Gets all farms a user has access to
        /// </summary>
        public async Task<List<UserFarmProfileViewModel>> GetUserFarmProfilesAsync(string userId = null)
        {
            try
            {
                var queryParam = string.IsNullOrEmpty(userId) ? "" : $"?userId={userId}";
                var response = await _apiCall.Get<ApiResponse<List<UserFarmProfileViewModel>>>(
                    await _apiCall.GetHttpClient(), $"farmusers/getuserfarmprofiles{queryParam}");

                if (response?.Status == (int)HttpStatusCode.OK && response.Result != null)
                {
                    return response.Result;
                }

                return new List<UserFarmProfileViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user farm profiles: {ex.Message}");
                return new List<UserFarmProfileViewModel>();
            }
        }

        /// <summary>
        /// Assigns a user to a farm with a specific role
        /// </summary>
        public async Task<(bool Success, string Message)> AssignUserToFarmAsync(string userId, string farmId, string role)
        {
            try
            {
                // Check if user has permission to manage users for this farm
                bool canManage = await _farmAccessService.CanManageFarm(farmId);
                if (!canManage)
                {
                    return (false, "You don't have permission to manage users for this farm");
                }

                var assignment = new FarmUserAssignmentViewModel
                {
                    UserId = userId,
                    FarmId = farmId,
                    Role = role
                };

                var response = await _apiCall.Add<ApiResponse<UserProfileViewModel>, FarmUserAssignmentViewModel>(
                    await _apiCall.GetHttpClient(), "farmusers/assignusertofarm", assignment);

                if (response?.Status == (int)HttpStatusCode.OK)
                {
                    return (true, response.Title ?? "User assigned to farm successfully");
                }

                return (false, response?.Title ?? "Failed to assign user to farm");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning user to farm: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a user's role or status for a specific farm
        /// </summary>
        public async Task<(bool Success, string Message)> UpdateUserFarmRoleAsync(string profileId, string newRole, bool isActive = true)
        {
            try
            {
                // Fetch profile to get farm ID
                var profiles = await GetUserFarmProfilesAsync();
                var profile = profiles.FirstOrDefault(p => p.ProfileId == profileId);
                
                if (profile == null)
                {
                    return (false, "Profile not found");
                }
                
                // Check if user has permission to manage users for this farm
                bool canManage = await _farmAccessService.CanManageFarm(profile.FarmId);
                if (!canManage)
                {
                    return (false, "You don't have permission to manage users for this farm");
                }

                var updateModel = new UpdateFarmUserViewModel
                {
                    ProfileId = profileId,
                    Role = newRole,
                    IsActive = isActive
                };

                var response = await _apiCall.Update<ApiResponse<UserProfileViewModel>, UpdateFarmUserViewModel>(
                    await _apiCall.GetHttpClient(), "farmusers/updateuserfarmrole", updateModel);

                if (response?.Status == (int)HttpStatusCode.OK)
                {
                    return (true, response.Title ?? "User role updated successfully");
                }

                return (false, response?.Title ?? "Failed to update user role");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user farm role: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a user from a farm
        /// </summary>
        public async Task<(bool Success, string Message)> RemoveUserFromFarmAsync(string userId, string farmId)
        {
            try
            {
                // Check if user has permission to manage users for this farm
                bool canManage = await _farmAccessService.CanManageFarm(farmId);
                if (!canManage)
                {
                    return (false, "You don't have permission to manage users for this farm");
                }

                var response = await _apiCall.Remove<ApiResponse<object>>(
                    await _apiCall.GetHttpClient(), "farmusers/removeuserfromfarm", 
                    $"{userId}?farmId={farmId}");

                if (response?.Status == (int)HttpStatusCode.OK)
                {
                    return (true, response.Title ?? "User removed from farm successfully");
                }

                return (false, response?.Title ?? "Failed to remove user from farm");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing user from farm: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all farms that the current user can access
        /// </summary>
        public async Task<List<FarmListViewModel>> GetCurrentUserAccessibleFarmsAsync()
        {
            try
            {
                var response = await _apiCall.Get<ApiResponse<List<FarmListViewModel>>>(
                    await _apiCall.GetHttpClient(), "farmusers/getaccessiblefarms");

                if (response?.Status == (int)HttpStatusCode.OK && response.Result != null)
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
        
        /// <summary>
        /// Gets user farm assignment statistics across the system
        /// </summary>
        public async Task<List<FarmUserSummaryViewModel>> GetUserAssignmentStatisticsAsync()
        {
            try
            {
                // Check if user is an admin
                var authState = await _authProvider.GetAuthenticationStateAsync();
                bool isAdmin = authState.User.IsInRole("SystemAdministrator");
                
                if (!isAdmin)
                {
                    return new List<FarmUserSummaryViewModel>();
                }
                
                var response = await _apiCall.Get<ApiResponse<List<FarmUserSummaryViewModel>>>(
                    await _apiCall.GetHttpClient(), "farmusers/getuserassignmentstatistics");

                if (response?.Status == (int)HttpStatusCode.OK && response.Result != null)
                {
                    return response.Result;
                }

                return new List<FarmUserSummaryViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user assignment statistics: {ex.Message}");
                return new List<FarmUserSummaryViewModel>();
            }
        }
    }
}