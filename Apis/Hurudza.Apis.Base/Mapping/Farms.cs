using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping;

public class Farms : Profile
{
    public Farms()
    {
        CreateMap<FarmViewModel, Farm>();
        CreateMap<Farm, FarmViewModel>()
            .ForMember(dst => dst.Province, opts => opts.MapFrom(src => src.Province.Name))
            .ForMember(dst => dst.District, opts => opts.MapFrom(src => src.District.Name))
            .ForMember(dst => dst.LocalAuthority, opts => opts.MapFrom(src => src.LocalAuthority.Name))
            .ForMember(dst => dst.Ward, opts => opts.MapFrom(src => src.Ward.Name));
    }
}