using Hurudza.Data.UI.Models.ViewModels.Base;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Data.UI.Models.ViewModels.UserManagement;

public class FarmUserViewModel : BaseViewModel
{
    // User Information
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    
    // Farm Assignment Information
    public string ProfileId { get; set; }
    public string FarmId { get; set; }
    public string FarmName { get; set; }
    public string Role { get; set; }
    public DateTime AssignedDate { get; set; }
    
    // Additional Properties
    public bool IsActive { get; set; } = true;
    public List<string> Permissions { get; set; } = new List<string>();
}

public class AssignFarmUserViewModel
{
    public required string UserId { get; set; }
    public required string FarmId { get; set; }
    public required string Role { get; set; }
}

public class UpdateFarmUserViewModel
{
    public required string ProfileId { get; set; }
    public required string Role { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FarmUserSummaryViewModel
{
    public string FarmId { get; set; }
    public string FarmName { get; set; }
    public int TotalUsers { get; set; }
    public int Administrators { get; set; }
    public int Managers { get; set; }
    public int FieldOfficers { get; set; }
    public int Viewers { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class UserFarmAccessViewModel
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Fullname { get; set; }
    public List<UserFarmProfileViewModel> FarmProfiles { get; set; } = new List<UserFarmProfileViewModel>();
    public bool IsSystemAdministrator { get; set; }
    public DateTime LastLogin { get; set; }
}

public class UserFarmProfileViewModel
{
    public string ProfileId { get; set; }
    public string FarmId { get; set; }
    public string FarmName { get; set; }
    public string Role { get; set; }
    public DateTime AssignedDate { get; set; }
    public bool IsActive { get; set; }
}