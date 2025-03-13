using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping;

public class FarmOwnership : Profile
{
    public FarmOwnership()
    {
        CreateMap<FarmOwner, FarmOwnerViewModel>()
            .ForMember(dest => dest.Farm, opt => opt.MapFrom(src => src.Farm.Name))
            .ForMember(dest => dest.Entity, opt => opt.MapFrom(src => src.Entity.Name))
            .ReverseMap();
        CreateMap<FarmOwner, CreateFarmOwnerViewModel>().ReverseMap();
    }
}