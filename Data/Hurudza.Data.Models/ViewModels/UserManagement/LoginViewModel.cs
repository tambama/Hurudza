using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.Models.ViewModels.UserManagement;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Username must be an email")]
    public string? Username { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public bool RememberMe { get; set; }
}