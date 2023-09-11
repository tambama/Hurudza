using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping
{
    public class Locations : Profile
    {
        public Locations()
        {
            CreateMap<Location, LocationViewModel>();

            CreateMap<FarmLocation, FarmLocationViewModel>()
                .ForMember(dst => dst.Farm, opts => opts.MapFrom(src => src.Farm.Name));

            CreateMap<FieldLocation, FieldLocationViewModel>()
                .ForMember(dst => dst.Field, opts => opts.MapFrom(src => src.Field.Name));
        }
    }
}
