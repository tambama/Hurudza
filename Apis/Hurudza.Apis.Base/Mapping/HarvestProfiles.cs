using AutoMapper;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Apis.Base.Mapping;

public class HarvestProfiles : Profile
{
    public HarvestProfiles()
    {
        // Harvest Plan mappings
        CreateMap<HarvestPlan, HarvestPlanViewModel>()
            .ForMember(dst => dst.Farm, opts => opts.MapFrom(src => src.Farm.Name));

        CreateMap<CreateHarvestPlanViewModel, HarvestPlan>()
            .ForMember(dst => dst.Id, opts => opts.Ignore());
        CreateMap<HarvestPlanViewModel, HarvestPlan>();

        // Harvest Schedule mappings
        CreateMap<HarvestSchedule, HarvestScheduleViewModel>()
            .ForMember(dst => dst.HarvestPlan, opts => opts.MapFrom(src => src.HarvestPlan.Season))
            .ForMember(dst => dst.Field, opts => opts.MapFrom(src => src.FieldCrop.Field.Name))
            .ForMember(dst => dst.Crop, opts => opts.MapFrom(src => src.FieldCrop.Crop.Name))
            .ForMember(dst => dst.Farm, opts => opts.MapFrom(src => src.HarvestPlan.Farm.Name));

        CreateMap<CreateHarvestScheduleViewModel, HarvestSchedule>()
            .ForMember(dst => dst.Id, opts => opts.Ignore());
        CreateMap<HarvestScheduleViewModel, HarvestSchedule>();

        // Harvest Record mappings
        CreateMap<HarvestRecord, HarvestRecordViewModel>()
            .ForMember(dst => dst.Field, opts => opts.MapFrom(src => src.HarvestSchedule.FieldCrop.Field.Name))
            .ForMember(dst => dst.Crop, opts => opts.MapFrom(src => src.HarvestSchedule.FieldCrop.Crop.Name))
            .ForMember(dst => dst.Farm, opts => opts.MapFrom(src => src.HarvestSchedule.HarvestPlan.Farm.Name))
            .ForMember(dst => dst.EstimatedYield, opts => opts.MapFrom(src => src.HarvestSchedule.EstimatedYield));

        CreateMap<CreateHarvestRecordViewModel, HarvestRecord>()
            .ForMember(dst => dst.Id, opts => opts.Ignore());
        CreateMap<HarvestRecordViewModel, HarvestRecord>();

        // Harvest Loss mappings
        CreateMap<HarvestLoss, HarvestLossViewModel>()
            .ForMember(dst => dst.Field, opts => opts.MapFrom(src => src.HarvestRecord.HarvestSchedule.FieldCrop.Field.Name))
            .ForMember(dst => dst.Crop, opts => opts.MapFrom(src => src.HarvestRecord.HarvestSchedule.FieldCrop.Crop.Name))
            .ForMember(dst => dst.TotalYield, opts => opts.MapFrom(src => src.HarvestRecord.ActualYield));

        CreateMap<CreateHarvestLossViewModel, HarvestLoss>()
            .ForMember(dst => dst.Id, opts => opts.Ignore());
    }
}