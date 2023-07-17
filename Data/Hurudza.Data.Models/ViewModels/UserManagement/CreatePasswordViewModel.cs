using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.Models.ViewModels.UserManagement;

public class CreatePasswordViewModel
{
    public bool HasPassword { get; set; }
    public required string RegistrationToken { get; set; }

    [Display(Name = "Password")]
    public string? Password { get; set; }

    [Display(Name = "Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "Make sure Password and Confirm Password match")]
    public string? ConfirmPassword { get; set; }
}
