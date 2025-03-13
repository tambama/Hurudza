using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping
{
    public class FieldCrops : Profile
    {
        public FieldCrops()
        {
            CreateMap<FieldCrop, FieldCropViewModel>()
                .ForMember(dst => dst.Crop, opts => opts.MapFrom(src => src.Crop.Name))
                .ForMember(dst => dst.Field, opts => opts.MapFrom(src => src.Field.Name));
        }
    }
}
