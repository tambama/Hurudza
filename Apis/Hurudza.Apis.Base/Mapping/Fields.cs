using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping;

public class Fields : Profile
{
    public Fields()
    {
        CreateMap<FieldViewModel, Field>();
        CreateMap<CreateFieldViewModel, Field>();
        CreateMap<Field, FieldViewModel>()
            .ForMember(dst => dst.Farm, opts => opts.MapFrom(src => src.Farm.Name))
            .ForMember(dst => dst.Locations, opts => opts.MapFrom(src => src.Locations.OrderBy(l => l.CreatedDate)));
    }
}