using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping;

public class Entities : Profile
{
    public Entities()
    {
        CreateMap<Entity, EntityViewModel>().ReverseMap();
        CreateMap<Entity, EntityListViewModel>();
    }
}