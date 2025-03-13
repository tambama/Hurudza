using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Hurudza.Data.UI.Models.ViewModels.Stats;

namespace Hurudza.Apis.Base.Mapping;

public class Farms : Profile
{
    public Farms()
    {
        CreateMap<Farm, FarmViewModel>()
            .ForMember(dst => dst.Province, opts => opts.MapFrom(src => src.Province.Name))
            .ForMember(dst => dst.District, opts => opts.MapFrom(src => src.District.Name))
            .ForMember(dst => dst.LocalAuthority, opts => opts.MapFrom(src => src.LocalAuthority.Name))
            .ForMember(dst => dst.Ward, opts => opts.MapFrom(src => src.Ward.Name));
        CreateMap<Farm, FarmMapViewModel>()
            .ForMember(dst => dst.FarmCoordinates, opts => opts.MapFrom(src => src.Locations.OrderBy(l => l.CreatedDate).Select(l => new List<double>() { l.Longitude, l.Latitude })));

        CreateMap<Farm, FarmStatisticsViewModel>();
        
        CreateMap<Farm, FarmListViewModel>();
    }
}