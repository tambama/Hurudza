namespace Hurudza.Data.Models.ViewModels.UserManagement;

public class UserProfileViewModel
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string Fullname { get; set; }

    public string FarmId { get; set; }

    public string Role { get; set; }

    public string Farm { get; set; }

    public bool LoggedIn { get; set; } = false;

    public bool IsFarm { get; set; } = true;
}
