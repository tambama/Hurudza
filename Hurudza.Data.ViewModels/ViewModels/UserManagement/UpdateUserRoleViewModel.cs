using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.UI.Models.ViewModels.UserManagement;

public class UpdateUserRoleViewModel
{
    [Required]
    public required string Id { get; set; }
    public string? FromRole { get; set; }
    [Required]
    public required string ToRole { get; set; }
    public required string FarmId { get; set; }
}
