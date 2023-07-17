using Hurudza.Data.Models.Models;

namespace Hurudza.Data.Models.ViewModels.UserManagement;

public class RoleViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class FilterRolesViewModel
{
    public List<RoleClass> RoleClasses { get; set; }
}