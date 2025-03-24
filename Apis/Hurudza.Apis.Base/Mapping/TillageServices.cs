using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Tillage;

namespace Hurudza.Apis.Base.Mapping;

public class TillageServices : Profile
{
    public TillageServices()
    {
        CreateMap<TillageService, TillageServiceViewModel>()
            .ForMember(dst => dst.TillageProgram, opts => opts.MapFrom(src => src.TillageProgram.Name))
            .ForMember(dst => dst.RecipientFarm, opts => opts.MapFrom(src => src.RecipientFarm.Name))
            .ForMember(dst => dst.Field, opts => opts.MapFrom(src => src.Field.Name));
            
        CreateMap<TillageServiceViewModel, TillageService>();
    }
}