using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping
{
    public class Provinces : Profile
    {
        public Provinces()
        {
            CreateMap<Province, ProvinceViewModel>();
        }
    }
}
