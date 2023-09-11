using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;

namespace Hurudza.Apis.Base.Mapping;

public class Roles: Profile
{
    public Roles()
    {
        CreateMap<ApplicationRole, RoleViewModel>();
    }
}