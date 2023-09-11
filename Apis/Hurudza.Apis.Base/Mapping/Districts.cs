using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping
{
    public class Districts : Profile
    {
        public Districts()
        {
            CreateMap<District, DistrictViewModel>()
                .ForMember(dst => dst.Province, opts => opts.MapFrom(src => src.Province.Name));
        }
    }
}
