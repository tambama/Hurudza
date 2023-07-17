using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.Models.ViewModels.UserManagement;

public class ResetPasswordViewModel
{
    [Required]
    public required string Code { get; set; }
    [Required]
    [Display(Name = "Code")]
    [Compare(nameof(Code), ErrorMessage = "Code does not match")]
    public string? ConfirmCode { get; set; }
    [Required]
    public string? NewPassword { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    public required string Token { get; set; }
    [Required]
    public DateTime? TokenValidity { get; set; }
}
