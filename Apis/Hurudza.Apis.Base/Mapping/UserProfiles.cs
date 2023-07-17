using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Models.ViewModels.UserManagement;

namespace Hurudza.Apis.Base.Mapping;

public class UserProfiles : Profile
{
    public UserProfiles()
    {
        CreateMap<UserProfile, UserProfileViewModel>()
            .ForMember(dst => dst.Fullname, opts => opts.MapFrom(src => src.User.Fullname));

        CreateMap<UserProfile, UserViewModel>()
            .ForMember(dst => dst.Id, opts => opts.MapFrom(src => src.UserId))
            .ForMember(dst => dst.ProfileId, opts => opts.MapFrom(src => src.Id))
            .ForMember(dst => dst.Email, opts => opts.MapFrom(src => src.User.Email))
            .ForMember(dst => dst.Firstname, opts => opts.MapFrom(src => src.User.Firstname))
            .ForMember(dst => dst.Surname, opts => opts.MapFrom(src => src.User.Surname))
            .ForMember(dst => dst.PhoneNumber, opts => opts.MapFrom(src => src.User.PhoneNumber))
            .ForMember(dst => dst.UserName, opts => opts.MapFrom(src => src.User.UserName));
    }
}