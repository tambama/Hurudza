using Hurudza.Data.UI.Models.ViewModels.UserManagement;

namespace Hurudza.Data.Services.Interfaces;

public interface IFarmUserAssignmentService
{
    Task<UserProfileViewModel> AssignUserToFarmAsync(string userId, string farmId, string role);
    Task RemoveUserFromFarmAsync(string userId, string farmId);
    Task<IEnumerable<UserProfileViewModel>> GetUserFarmProfilesAsync(string userId);
    Task<IEnumerable<UserProfileViewModel>> GetFarmUsersAsync(string farmId);
    Task<UserProfileViewModel> UpdateUserFarmRoleAsync(string profileId, string newRole);
    Task<IEnumerable<string>> GetUserAccessibleFarmIdsAsync(string userId);
}