namespace Hurudza.Data.Models.ViewModels.UserManagement;

public class UserViewModel
{
    public required string Id { get; set; }
    public required string Firstname { get; set; }
    public required string Surname { get; set; }
    public required string UserName { get; set; }
    public required string PhoneNumber { get; set; }
    public string? ProfileId { get; set; }
    public required string Email { get; set; }
    public string? Role { get; set; }
    public int TotalProfiles { get; set; }
    public string? Token { get; set; }
    public bool IsAdmin { get; set; }
    public string? FarmId { get; set; }
    public List<UserProfileViewModel>? Profiles { get; set; } = new List<UserProfileViewModel>();
}
