using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.ViewModels.UserManagement;

public class RoleViewModel
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public ICollection<ClaimViewModel> Permissions { get; set; } = new List<ClaimViewModel>();
}

public class FilterRolesViewModel
{
    public List<RoleClass> RoleClasses { get; set; }
}