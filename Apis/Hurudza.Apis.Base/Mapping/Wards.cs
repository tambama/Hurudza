using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping
{
    public class Wards : Profile
    {
        public Wards()
        {
            CreateMap<Ward, WardViewModel>()
                .ForMember(dst => dst.District, opts => opts.MapFrom(src => src.District.Name))
                .ForMember(dst => dst.Province, opts => opts.MapFrom(src => src.Province.Name))
                .ForMember(dst => dst.LocalAuthority, opts => opts.MapFrom(src => src.LocalAuthority.Name));
        }
    }
}
