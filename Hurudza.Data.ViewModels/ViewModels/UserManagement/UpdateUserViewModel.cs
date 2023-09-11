using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.ViewModels.UserManagement;

public class UpdateUserViewModel
{
    public required string Id { get; set; }
    [Display(Name = "Username")]
    public required string UserName { get; set; }
    [Required]
    [Display(Name = "Firstname")]
    public string Firstname { get; set; }
    [Required]
    [Display(Name = "Surname")]
    public required string Surname { get; set; }
    [Required]
    [Display(Name = "Mobile Number")]
    [RegularExpression(@"^((00|\+)?(263))?0?7(1|3|7|8)[0-9]{7}$", ErrorMessage = "Enter a Zimbabwean Number")]
    public string PhoneNumber { get; set; }
    public Role Role { get; set; }
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email (optional)")]
    public string Email { get; set; }
    public string Password { get; set; }
    [Required]
    public string FarmId { get; set; }
}