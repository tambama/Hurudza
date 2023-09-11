using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping
{
    public class LocalAuthorities : Profile
    {
        public LocalAuthorities()
        {
            CreateMap<LocalAuthority, LocalAuthorityViewModel>()
                .ForMember(dst => dst.District, opts => opts.MapFrom(src => src.District.Name));
        }
    }
}
