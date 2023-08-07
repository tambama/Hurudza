using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping;

public class Crops : Profile
{
    public Crops()
    {
        CreateMap<FieldCrop, FieldCropViewModel>()
            .ForMember(dst => dst.Field, opts => opts.MapFrom(src => src.Field.Name))
            .ForMember(dst => dst.Crop, opts => opts.MapFrom(src => src.Crop.Name));
    }
}