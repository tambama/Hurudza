using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;

namespace Hurudza.Apis.Base.Mapping;

public class Users : Profile
{
    public Users()
    {
        CreateMap<ApplicationUser, UserViewModel>();
    }
}