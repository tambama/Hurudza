using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.Models.ViewModels.UserManagement;

public class UserViewModel
{
    public required string Id { get; set; }
    [Required(ErrorMessage = "Firstname is required")]
    public required string Firstname { get; set; }
    [Required(ErrorMessage = "Surname is required")]
    public required string Surname { get; set; }
    public string Fullname => $"{Firstname} {Surname}".Trim();
    public required string UserName { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^((00|\+)?(263))?0?7(1|3|7|8)[0-9]{7}$", ErrorMessage = "Enter a valid mobile number")]
    public required string PhoneNumber { get; set; }
    public string? ProfileId { get; set; }
    
    [Required(ErrorMessage = "Email address is required")]
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Enter a valid email address")]
    public required string Email { get; set; }
    
    [Required(ErrorMessage = "Select a role")]
    public string? Role { get; set; }
    
    public int TotalProfiles { get; set; }
    public string? Token { get; set; }
    public bool IsAdmin { get; set; }
    public string? Farm { get; set; }
    public string? FarmId { get; set; }
    public List<UserProfileViewModel>? Profiles { get; set; } = new List<UserProfileViewModel>();
}
