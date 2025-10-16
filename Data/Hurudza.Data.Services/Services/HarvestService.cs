using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hurudza.Data.Services.Services;

public class HarvestService : IHarvestService
{
    private readonly HurudzaDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<HarvestService> _logger;

    public HarvestService(
        HurudzaDbContext context, 
        IMapper mapper, 
        ILogger<HarvestService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    #region Harvest Plans

    public async Task<List<HarvestPlanViewModel>> GetAllHarvestPlansAsync()
    {
        try
        {
            return await _context.HarvestPlans
                .Include(hp => hp.Farm)
                .Include(hp => hp.HarvestSchedules)
                .Where(hp => hp.IsActive && !hp.Deleted)
                .ProjectTo<HarvestPlanViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest plans");
            return new List<HarvestPlanViewModel>();
        }
    }

    public async Task<List<HarvestPlanViewModel>> GetHarvestPlansByFarmAsync(string farmId)
    {
        try
        {
            return await _context.HarvestPlans
                .Include(hp => hp.Farm)
                .Include(hp => hp.HarvestSchedules)
                .Where(hp => hp.IsActive && !hp.Deleted && hp.FarmId == farmId)
                .ProjectTo<HarvestPlanViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest plans for farm {FarmId}", farmId);
            return new List<HarvestPlanViewModel>();
        }
    }

    public async Task<HarvestPlanViewModel?> GetHarvestPlanByIdAsync(string id)
    {
        try
        {
            return await _context.HarvestPlans
                .Include(hp => hp.Farm)
                .Include(hp => hp.HarvestSchedules)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Field)
                .Include(hp => hp.HarvestSchedules)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Crop)
                .Where(hp => hp.Id == id && hp.IsActive && !hp.Deleted)
                .ProjectTo<HarvestPlanViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest plan {Id}", id);
            return null;
        }
    }

    public async Task<HarvestPlanViewModel> CreateHarvestPlanAsync(CreateHarvestPlanViewModel model)
    {
        try
        {
            var harvestPlan = _mapper.Map<HarvestPlan>(model);
            harvestPlan.Status = HarvestPlanStatus.Draft;

            await _context.HarvestPlans.AddAsync(harvestPlan);
            await _context.SaveChangesAsync();

            return _mapper.Map<HarvestPlanViewModel>(harvestPlan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating harvest plan");
            throw;
        }
    }

    public async Task<HarvestPlanViewModel?> UpdateHarvestPlanAsync(string id, HarvestPlanViewModel model)
    {
        try
        {
            var existingPlan = await _context.HarvestPlans
                .FirstOrDefaultAsync(hp => hp.Id == id && hp.IsActive && !hp.Deleted);

            if (existingPlan == null)
                return null;

            _mapper.Map(model, existingPlan);
            await _context.SaveChangesAsync();

            return _mapper.Map<HarvestPlanViewModel>(existingPlan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating harvest plan {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteHarvestPlanAsync(string id)
    {
        try
        {
            var harvestPlan = await _context.HarvestPlans
                .FirstOrDefaultAsync(hp => hp.Id == id);

            if (harvestPlan == null)
                return false;

            harvestPlan.Deleted = true;
            harvestPlan.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting harvest plan {Id}", id);
            return false;
        }
    }

    #endregion

    #region Harvest Schedules

    public async Task<List<HarvestScheduleViewModel>> GetAllHarvestSchedulesAsync()
    {
        try
        {
            var schedules = await _context.HarvestSchedules
                .Include(hs => hs.HarvestPlan)
                    .ThenInclude(hp => hp.Farm)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Field)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Crop)
                .Include(hs => hs.HarvestRecords)
                .Where(hs => hs.IsActive && !hs.Deleted)
                .ToListAsync();

            return _mapper.Map<List<HarvestScheduleViewModel>>(schedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest schedules");
            return new List<HarvestScheduleViewModel>();
        }
    }

    public async Task<List<HarvestScheduleViewModel>> GetHarvestSchedulesByPlanAsync(string planId)
    {
        try
        {
            var schedules = await _context.HarvestSchedules
                .Include(hs => hs.HarvestPlan)
                    .ThenInclude(hp => hp.Farm)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Field)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Crop)
                .Include(hs => hs.HarvestRecords)
                .Where(hs => hs.HarvestPlanId == planId && hs.IsActive && !hs.Deleted)
                .ToListAsync();

            return _mapper.Map<List<HarvestScheduleViewModel>>(schedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest schedules for plan {PlanId}", planId);
            return new List<HarvestScheduleViewModel>();
        }
    }

    public async Task<List<HarvestScheduleViewModel>> GetHarvestSchedulesByFarmAsync(string farmId)
    {
        try
        {
            var schedules = await _context.HarvestSchedules
                .Include(hs => hs.HarvestPlan)
                    .ThenInclude(hp => hp.Farm)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Field)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Crop)
                .Include(hs => hs.HarvestRecords)
                .Where(hs => hs.HarvestPlan.FarmId == farmId && hs.IsActive && !hs.Deleted)
                .ToListAsync();

            return _mapper.Map<List<HarvestScheduleViewModel>>(schedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest schedules for farm {FarmId}", farmId);
            return new List<HarvestScheduleViewModel>();
        }
    }

    public async Task<List<HarvestScheduleViewModel>> GetUpcomingHarvestsAsync(string? farmId = null, int days = 30)
    {
        try
        {
            var cutoffDate = DateTime.Now.AddDays(days);
            var query = _context.HarvestSchedules
                .Include(hs => hs.HarvestPlan)
                    .ThenInclude(hp => hp.Farm)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Field)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Crop)
                .Include(hs => hs.HarvestRecords)
                .Where(hs => hs.IsActive && !hs.Deleted
                    && hs.PlannedDate.Date >= DateTime.Now.Date
                    && hs.PlannedDate <= cutoffDate
                    && (hs.Status == HarvestStatus.Planned || hs.Status == HarvestStatus.InProgress));

            if (!string.IsNullOrEmpty(farmId))
            {
                query = query.Where(hs => hs.HarvestPlan.FarmId == farmId);
            }

            var schedules = await query
                .OrderBy(hs => hs.PlannedDate)
                .ToListAsync();

            return _mapper.Map<List<HarvestScheduleViewModel>>(schedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming harvests");
            return new List<HarvestScheduleViewModel>();
        }
    }

    public async Task<List<HarvestScheduleViewModel>> GetOverdueHarvestsAsync(string? farmId = null)
    {
        try
        {
            var query = _context.HarvestSchedules
                .Include(hs => hs.HarvestPlan)
                    .ThenInclude(hp => hp.Farm)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Field)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Crop)
                .Include(hs => hs.HarvestRecords)
                .Where(hs => hs.IsActive && !hs.Deleted
                    && hs.PlannedDate < DateTime.Now
                    && (hs.Status == HarvestStatus.Planned || hs.Status == HarvestStatus.InProgress));

            if (!string.IsNullOrEmpty(farmId))
            {
                query = query.Where(hs => hs.HarvestPlan.FarmId == farmId);
            }

            var schedules = await query
                .OrderBy(hs => hs.PlannedDate)
                .ToListAsync();

            return _mapper.Map<List<HarvestScheduleViewModel>>(schedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue harvests");
            return new List<HarvestScheduleViewModel>();
        }
    }

    public async Task<HarvestScheduleViewModel?> GetHarvestScheduleByIdAsync(string id)
    {
        try
        {
            var schedule = await _context.HarvestSchedules
                .Include(hs => hs.HarvestPlan)
                    .ThenInclude(hp => hp.Farm)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Field)
                .Include(hs => hs.FieldCrop)
                    .ThenInclude(fc => fc.Crop)
                .Include(hs => hs.HarvestRecords)
                .Where(hs => hs.Id == id && hs.IsActive && !hs.Deleted)
                .FirstOrDefaultAsync();

            return schedule == null ? null : _mapper.Map<HarvestScheduleViewModel>(schedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest schedule {Id}", id);
            return null;
        }
    }

    public async Task<HarvestScheduleViewModel> CreateHarvestScheduleAsync(CreateHarvestScheduleViewModel model)
    {
        try
        {
            var harvestSchedule = _mapper.Map<HarvestSchedule>(model);
            harvestSchedule.Status = HarvestStatus.Planned;

            await _context.HarvestSchedules.AddAsync(harvestSchedule);
            await _context.SaveChangesAsync();

            return _mapper.Map<HarvestScheduleViewModel>(harvestSchedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating harvest schedule");
            throw;
        }
    }

    public async Task<HarvestScheduleViewModel?> UpdateHarvestScheduleAsync(string id, HarvestScheduleViewModel model)
    {
        try
        {
            var existingSchedule = await _context.HarvestSchedules
                .FirstOrDefaultAsync(hs => hs.Id == id && hs.IsActive && !hs.Deleted);

            if (existingSchedule == null)
                return null;

            _mapper.Map(model, existingSchedule);
            await _context.SaveChangesAsync();

            return _mapper.Map<HarvestScheduleViewModel>(existingSchedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating harvest schedule {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteHarvestScheduleAsync(string id)
    {
        try
        {
            var harvestSchedule = await _context.HarvestSchedules
                .FirstOrDefaultAsync(hs => hs.Id == id);

            if (harvestSchedule == null)
                return false;

            harvestSchedule.Deleted = true;
            harvestSchedule.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting harvest schedule {Id}", id);
            return false;
        }
    }

    #endregion

    #region Harvest Records

    public async Task<List<HarvestRecordViewModel>> GetAllHarvestRecordsAsync()
    {
        try
        {
            return await _context.HarvestRecords
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Field)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Crop)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.HarvestPlan)
                        .ThenInclude(hp => hp.Farm)
                .Include(hr => hr.HarvestLosses)
                .Where(hr => hr.IsActive && !hr.Deleted)
                .ProjectTo<HarvestRecordViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest records");
            return new List<HarvestRecordViewModel>();
        }
    }

    public async Task<List<HarvestRecordViewModel>> GetHarvestRecordsByScheduleAsync(string scheduleId)
    {
        try
        {
            return await _context.HarvestRecords
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Field)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Crop)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.HarvestPlan)
                        .ThenInclude(hp => hp.Farm)
                .Include(hr => hr.HarvestLosses)
                .Where(hr => hr.HarvestScheduleId == scheduleId && hr.IsActive && !hr.Deleted)
                .ProjectTo<HarvestRecordViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest records for schedule {ScheduleId}", scheduleId);
            return new List<HarvestRecordViewModel>();
        }
    }

    public async Task<List<HarvestRecordViewModel>> GetHarvestRecordsByFarmAsync(string farmId)
    {
        try
        {
            return await _context.HarvestRecords
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Field)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Crop)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.HarvestPlan)
                        .ThenInclude(hp => hp.Farm)
                .Include(hr => hr.HarvestLosses)
                .Where(hr => hr.HarvestSchedule.HarvestPlan.FarmId == farmId && hr.IsActive && !hr.Deleted)
                .ProjectTo<HarvestRecordViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest records for farm {FarmId}", farmId);
            return new List<HarvestRecordViewModel>();
        }
    }

    public async Task<HarvestRecordViewModel?> GetHarvestRecordByIdAsync(string id)
    {
        try
        {
            return await _context.HarvestRecords
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Field)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Crop)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.HarvestPlan)
                        .ThenInclude(hp => hp.Farm)
                .Include(hr => hr.HarvestLosses)
                .Where(hr => hr.Id == id && hr.IsActive && !hr.Deleted)
                .ProjectTo<HarvestRecordViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest record {Id}", id);
            return null;
        }
    }

    public async Task<HarvestRecordViewModel> CreateHarvestRecordAsync(CreateHarvestRecordViewModel model)
    {
        try
        {
            var harvestRecord = _mapper.Map<HarvestRecord>(model);

            await _context.HarvestRecords.AddAsync(harvestRecord);

            // Update the harvest schedule status based on whether this is a complete harvest
            var schedule = await _context.HarvestSchedules
                .FirstOrDefaultAsync(hs => hs.Id == model.HarvestScheduleId);

            if (schedule != null)
            {
                schedule.ActualDate = model.HarvestDate;

                // If this is marked as a complete harvest, set status to Completed
                // Otherwise, set to InProgress to allow for additional partial harvests
                schedule.Status = model.IsCompleteHarvest ? HarvestStatus.Completed : HarvestStatus.InProgress;
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<HarvestRecordViewModel>(harvestRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating harvest record");
            throw;
        }
    }

    public async Task<HarvestRecordViewModel?> UpdateHarvestRecordAsync(string id, HarvestRecordViewModel model)
    {
        try
        {
            var existingRecord = await _context.HarvestRecords
                .FirstOrDefaultAsync(hr => hr.Id == id && hr.IsActive && !hr.Deleted);

            if (existingRecord == null)
                return null;

            _mapper.Map(model, existingRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<HarvestRecordViewModel>(existingRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating harvest record {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteHarvestRecordAsync(string id)
    {
        try
        {
            var harvestRecord = await _context.HarvestRecords
                .FirstOrDefaultAsync(hr => hr.Id == id);

            if (harvestRecord == null)
                return false;

            harvestRecord.Deleted = true;
            harvestRecord.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting harvest record {Id}", id);
            return false;
        }
    }

    #endregion

    #region Harvest Losses

    public async Task<List<HarvestLossViewModel>> GetHarvestLossesByRecordAsync(string recordId)
    {
        try
        {
            return await _context.HarvestLosses
                .Include(hl => hl.HarvestRecord)
                    .ThenInclude(hr => hr.HarvestSchedule)
                        .ThenInclude(hs => hs.FieldCrop)
                            .ThenInclude(fc => fc.Field)
                .Include(hl => hl.HarvestRecord)
                    .ThenInclude(hr => hr.HarvestSchedule)
                        .ThenInclude(hs => hs.FieldCrop)
                            .ThenInclude(fc => fc.Crop)
                .Where(hl => hl.HarvestRecordId == recordId && hl.IsActive && !hl.Deleted)
                .ProjectTo<HarvestLossViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest losses for record {RecordId}", recordId);
            return new List<HarvestLossViewModel>();
        }
    }

    public async Task<List<HarvestLossViewModel>> GetHarvestLossesByFarmAsync(string farmId)
    {
        try
        {
            return await _context.HarvestLosses
                .Include(hl => hl.HarvestRecord)
                    .ThenInclude(hr => hr.HarvestSchedule)
                        .ThenInclude(hs => hs.FieldCrop)
                            .ThenInclude(fc => fc.Field)
                .Include(hl => hl.HarvestRecord)
                    .ThenInclude(hr => hr.HarvestSchedule)
                        .ThenInclude(hs => hs.FieldCrop)
                            .ThenInclude(fc => fc.Crop)
                .Where(hl => hl.HarvestRecord.HarvestSchedule.HarvestPlan.FarmId == farmId 
                    && hl.IsActive && !hl.Deleted)
                .ProjectTo<HarvestLossViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest losses for farm {FarmId}", farmId);
            return new List<HarvestLossViewModel>();
        }
    }

    public async Task<HarvestLossViewModel> CreateHarvestLossAsync(CreateHarvestLossViewModel model)
    {
        try
        {
            var harvestLoss = _mapper.Map<HarvestLoss>(model);

            await _context.HarvestLosses.AddAsync(harvestLoss);
            await _context.SaveChangesAsync();

            return _mapper.Map<HarvestLossViewModel>(harvestLoss);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating harvest loss");
            throw;
        }
    }

    public async Task<bool> DeleteHarvestLossAsync(string id)
    {
        try
        {
            var harvestLoss = await _context.HarvestLosses
                .FirstOrDefaultAsync(hl => hl.Id == id);

            if (harvestLoss == null)
                return false;

            harvestLoss.Deleted = true;
            harvestLoss.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting harvest loss {Id}", id);
            return false;
        }
    }

    #endregion

    #region Analytics and Reports

    public async Task<HarvestAnalyticsViewModel> GetHarvestAnalyticsAsync(string farmId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = _context.HarvestRecords
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Crop)
                .Include(hr => hr.HarvestLosses)
                .Where(hr => hr.HarvestSchedule.HarvestPlan.FarmId == farmId && hr.IsActive && !hr.Deleted);

            if (startDate.HasValue)
                query = query.Where(hr => hr.HarvestDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(hr => hr.HarvestDate <= endDate.Value);

            var records = await query.ToListAsync();

            var analytics = new HarvestAnalyticsViewModel
            {
                TotalHarvests = records.Count,
                TotalYield = records.Sum(r => r.ActualYield),
                AverageYield = records.Any() ? records.Average(r => r.ActualYield) : 0,
                TotalLosses = records.SelectMany(r => r.HarvestLosses ?? new List<HarvestLoss>()).Sum(l => l.Quantity),
                YieldByCrop = records.GroupBy(r => r.HarvestSchedule.FieldCrop.Crop.Name)
                    .Select(g => new YieldByCropViewModel { Crop = g.Key, TotalYield = g.Sum(r => r.ActualYield) })
                    .ToList(),
                LossByType = records.SelectMany(r => r.HarvestLosses ?? new List<HarvestLoss>())
                    .GroupBy(l => l.LossType)
                    .Select(g => new LossByTypeViewModel { LossType = g.Key.ToString(), TotalLoss = g.Sum(l => l.Quantity) })
                    .ToList()
            };

            return analytics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest analytics for farm {FarmId}", farmId);
            return new HarvestAnalyticsViewModel();
        }
    }

    public async Task<List<HarvestRecordViewModel>> GetHarvestHistoryAsync(string farmId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = _context.HarvestRecords
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Field)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Crop)
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.HarvestPlan)
                        .ThenInclude(hp => hp.Farm)
                .Include(hr => hr.HarvestLosses)
                .Where(hr => hr.HarvestSchedule.HarvestPlan.FarmId == farmId && hr.IsActive && !hr.Deleted);

            if (startDate.HasValue)
                query = query.Where(hr => hr.HarvestDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(hr => hr.HarvestDate <= endDate.Value);

            return await query
                .OrderByDescending(hr => hr.HarvestDate)
                .ProjectTo<HarvestRecordViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving harvest history for farm {FarmId}", farmId);
            return new List<HarvestRecordViewModel>();
        }
    }

    public async Task<YieldComparisonViewModel> GetYieldComparisonAsync(string farmId, string? cropId = null)
    {
        try
        {
            var query = _context.HarvestRecords
                .Include(hr => hr.HarvestSchedule)
                    .ThenInclude(hs => hs.FieldCrop)
                        .ThenInclude(fc => fc.Crop)
                .Where(hr => hr.HarvestSchedule.HarvestPlan.FarmId == farmId && hr.IsActive && !hr.Deleted);

            if (!string.IsNullOrEmpty(cropId))
                query = query.Where(hr => hr.HarvestSchedule.FieldCrop.CropId == cropId);

            var records = await query.ToListAsync();

            var comparisons = records.GroupBy(r => new
            {
                r.HarvestSchedule.FieldCrop.CropId,
                r.HarvestSchedule.FieldCrop.Crop.Name,
                Year = r.HarvestDate.Year
            })
            .Select(g => new CropYieldComparisonViewModel
            {
                CropId = g.Key.CropId,
                Name = g.Key.Name,
                Year = g.Key.Year,
                TotalYield = g.Sum(r => r.ActualYield),
                AverageYield = g.Average(r => r.ActualYield),
                EstimatedYield = g.Sum(r => r.HarvestSchedule.EstimatedYield),
                YieldVariance = g.Sum(r => r.ActualYield) - g.Sum(r => r.HarvestSchedule.EstimatedYield)
            })
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Year)
            .ToList();

            return new YieldComparisonViewModel { Comparisons = comparisons };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving yield comparison for farm {FarmId}", farmId);
            return new YieldComparisonViewModel();
        }
    }

    #endregion
}