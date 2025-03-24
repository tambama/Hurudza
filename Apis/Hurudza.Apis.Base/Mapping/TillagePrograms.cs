using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Tillage;

namespace Hurudza.Apis.Base.Mapping;

public class TillagePrograms : Profile
{
    public TillagePrograms()
    {
        CreateMap<TillageProgram, TillageProgramViewModel>()
            .ForMember(dst => dst.Farm, opts => opts.MapFrom(src => src.Farm.Name));
            
        CreateMap<TillageProgramViewModel, TillageProgram>();
    }
}