using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hurudza.Data.Services.Interfaces
{
    /// <summary>
    /// Interface for managing farm user assignments
    /// </summary>
    public interface IFarmUserManagerService
    {
        /// <summary>
        /// Gets all users assigned to a farm with their roles and permissions
        /// </summary>
        Task<List<FarmUserViewModel>> GetFarmUsersAsync(string farmId);
        
        /// <summary>
        /// Gets a summary of users for a farm
        /// </summary>
        Task<FarmUserSummaryViewModel> GetFarmUserSummaryAsync(string farmId);
        
        /// <summary>
        /// Gets all farms a user has access to
        /// </summary>
        Task<List<UserFarmProfileViewModel>> GetUserFarmProfilesAsync(string userId);
        
        /// <summary>
        /// Gets detailed information about a user's access across farms
        /// </summary>
        Task<UserFarmAccessViewModel> GetUserFarmAccessAsync(string userId);
        
        /// <summary>
        /// Assigns a user to a farm with the specified role
        /// </summary>
        Task<UserProfileViewModel> AssignUserToFarmAsync(string userId, string farmId, string role);
        
        /// <summary>
        /// Updates a user's role for a specific farm
        /// </summary>
        Task<UserProfileViewModel> UpdateUserFarmRoleAsync(string profileId, string newRole, bool isActive = true);
        
        /// <summary>
        /// Updates a user's active status for a specific farm
        /// </summary>
        Task<UserProfileViewModel> UpdateUserFarmStatusAsync(string profileId, bool isActive);
        
        /// <summary>
        /// Removes a user's assignment from a farm
        /// </summary>
        Task RemoveUserFromFarmAsync(string userId, string farmId);
        
        /// <summary>
        /// Gets all farm IDs that a user has access to
        /// </summary>
        Task<List<string>> GetUserAccessibleFarmIdsAsync(string userId);
        
        /// <summary>
        /// Gets statistics about user assignments across all farms
        /// </summary>
        Task<List<FarmUserSummaryViewModel>> GetSystemUserAssignmentStatisticsAsync();
    }
}