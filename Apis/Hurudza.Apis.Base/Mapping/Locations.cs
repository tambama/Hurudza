using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping
{
    public class Locations : Profile
    {
        public Locations()
        {
            CreateMap<FieldLocation, FieldLocationViewModel>().ReverseMap();
            CreateMap<FarmLocation, FarmLocationViewModel>().ReverseMap();
            CreateMap<FieldLocation, CoordinateViewModel>().ReverseMap();
            CreateMap<FarmLocation, CoordinateViewModel>().ReverseMap();
        }
    }
}
