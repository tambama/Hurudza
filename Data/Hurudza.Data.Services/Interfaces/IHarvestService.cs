using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Data.Services.Interfaces;

public interface IHarvestService
{
    // Harvest Plans
    Task<List<HarvestPlanViewModel>> GetAllHarvestPlansAsync();
    Task<List<HarvestPlanViewModel>> GetHarvestPlansByFarmAsync(string farmId);
    Task<HarvestPlanViewModel?> GetHarvestPlanByIdAsync(string id);
    Task<HarvestPlanViewModel> CreateHarvestPlanAsync(CreateHarvestPlanViewModel model);
    Task<HarvestPlanViewModel?> UpdateHarvestPlanAsync(string id, HarvestPlanViewModel model);
    Task<bool> DeleteHarvestPlanAsync(string id);
    
    // Harvest Schedules
    Task<List<HarvestScheduleViewModel>> GetAllHarvestSchedulesAsync();
    Task<List<HarvestScheduleViewModel>> GetHarvestSchedulesByPlanAsync(string planId);
    Task<List<HarvestScheduleViewModel>> GetHarvestSchedulesByFarmAsync(string farmId);
    Task<List<HarvestScheduleViewModel>> GetUpcomingHarvestsAsync(string? farmId = null, int days = 30);
    Task<List<HarvestScheduleViewModel>> GetOverdueHarvestsAsync(string? farmId = null);
    Task<HarvestScheduleViewModel?> GetHarvestScheduleByIdAsync(string id);
    Task<HarvestScheduleViewModel> CreateHarvestScheduleAsync(CreateHarvestScheduleViewModel model);
    Task<HarvestScheduleViewModel?> UpdateHarvestScheduleAsync(string id, HarvestScheduleViewModel model);
    Task<bool> DeleteHarvestScheduleAsync(string id);
    
    // Harvest Records
    Task<List<HarvestRecordViewModel>> GetAllHarvestRecordsAsync();
    Task<List<HarvestRecordViewModel>> GetHarvestRecordsByScheduleAsync(string scheduleId);
    Task<List<HarvestRecordViewModel>> GetHarvestRecordsByFarmAsync(string farmId);
    Task<HarvestRecordViewModel?> GetHarvestRecordByIdAsync(string id);
    Task<HarvestRecordViewModel> CreateHarvestRecordAsync(CreateHarvestRecordViewModel model);
    Task<HarvestRecordViewModel?> UpdateHarvestRecordAsync(string id, HarvestRecordViewModel model);
    Task<bool> DeleteHarvestRecordAsync(string id);
    
    // Harvest Losses
    Task<List<HarvestLossViewModel>> GetHarvestLossesByRecordAsync(string recordId);
    Task<List<HarvestLossViewModel>> GetHarvestLossesByFarmAsync(string farmId);
    Task<HarvestLossViewModel> CreateHarvestLossAsync(CreateHarvestLossViewModel model);
    Task<bool> DeleteHarvestLossAsync(string id);
    
    // Analytics and Reports
    Task<HarvestAnalyticsViewModel> GetHarvestAnalyticsAsync(string farmId, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<HarvestRecordViewModel>> GetHarvestHistoryAsync(string farmId, DateTime? startDate = null, DateTime? endDate = null);
    Task<YieldComparisonViewModel> GetYieldComparisonAsync(string farmId, string? cropId = null);
}