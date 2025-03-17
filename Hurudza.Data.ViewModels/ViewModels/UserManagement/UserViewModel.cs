using Hurudza.Data.UI.Models.ViewModels.Base;
using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.UI.Models.ViewModels.UserManagement;

public class UserViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Firstname is required")]
    public string Firstname { get; set; }
    [Required(ErrorMessage = "Surname is required")]
    public string Surname { get; set; }
    public string Fullname => $"{Firstname} {Surname}".Trim();
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^((00|\+)?(263))?0?7(1|3|7|8)[0-9]{7}$", ErrorMessage = "Enter a valid mobile number")]
    public string PhoneNumber { get; set; }
    public string? ProfileId { get; set; }

    [Required(ErrorMessage = "Email address is required")]
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Select a role")]
    public string? Role { get; set; }

    public int TotalProfiles { get; set; }
    public string? Token { get; set; }
    public bool IsAdmin { get; set; }
    public string? Farm { get; set; }
    public string? FarmId { get; set; }
    public List<UserProfileViewModel>? Profiles { get; set; } = new List<UserProfileViewModel>();
    
    // New properties for enhanced authorization
    public List<ClaimViewModel>? Permissions { get; set; } = new List<ClaimViewModel>();
    
    // Helper methods to check permissions
    public bool HasPermission(string permissionName)
    {
        if (Permissions == null) return false;
        return Permissions.Any(p => p.ClaimValue == permissionName);
    }
    
    public bool HasAnyPermission(params string[] permissionNames)
    {
        if (Permissions == null) return false;
        return Permissions.Any(p => permissionNames.Contains(p.ClaimValue));
    }
    
    public bool HasAllPermissions(params string[] permissionNames)
    {
        if (Permissions == null) return false;
        var userPermissions = Permissions.Select(p => p.ClaimValue).ToList();
        return permissionNames.All(p => userPermissions.Contains(p));
    }
}