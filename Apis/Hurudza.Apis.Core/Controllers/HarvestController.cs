using System.Net;
using System.Net.Mime;
using Asp.Versioning;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Microsoft.AspNetCore.Mvc;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class HarvestController : Controller
{
    private readonly IHarvestService _harvestService;

    public HarvestController(IHarvestService harvestService)
    {
        _harvestService = harvestService;
    }

    #region Harvest Plans

    [HttpGet(Name = nameof(GetHarvestPlans))]
    public async Task<IActionResult> GetHarvestPlans()
    {
        var harvestPlans = await _harvestService.GetAllHarvestPlansAsync();
        return Ok(harvestPlans);
    }

    [HttpGet("{farmId}", Name = nameof(GetHarvestPlansByFarm))]
    public async Task<IActionResult> GetHarvestPlansByFarm(string farmId)
    {
        var harvestPlans = await _harvestService.GetHarvestPlansByFarmAsync(farmId);
        return Ok(harvestPlans);
    }

    [HttpGet("{id}", Name = nameof(GetHarvestPlanDetails))]
    public async Task<IActionResult> GetHarvestPlanDetails(string id)
    {
        var harvestPlan = await _harvestService.GetHarvestPlanByIdAsync(id);
        
        return Ok(harvestPlan == null 
            ? new ApiResponse((int)HttpStatusCode.NotFound, "Harvest plan not found") 
            : new ApiOkResponse(harvestPlan));
    }

    [HttpPost(Name = nameof(CreateHarvestPlan))]
    public async Task<IActionResult> CreateHarvestPlan([FromBody] CreateHarvestPlanViewModel model)
    {
        try
        {
            var harvestPlan = await _harvestService.CreateHarvestPlanAsync(model);
            return Ok(new ApiOkResponse(harvestPlan, "Harvest plan created successfully"));
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, $"Error creating harvest plan: {ex.Message}"));
        }
    }

    [HttpPut("{id}", Name = nameof(UpdateHarvestPlan))]
    public async Task<IActionResult> UpdateHarvestPlan(string id, [FromBody] HarvestPlanViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        try
        {
            var harvestPlan = await _harvestService.UpdateHarvestPlanAsync(id, model);
            
            return Ok(harvestPlan == null 
                ? new ApiResponse((int)HttpStatusCode.NotFound, "Harvest plan not found") 
                : new ApiOkResponse(harvestPlan, "Harvest plan updated successfully"));
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, $"Error updating harvest plan: {ex.Message}"));
        }
    }

    [HttpDelete("{id}", Name = nameof(DeleteHarvestPlan))]
    public async Task<IActionResult> DeleteHarvestPlan(string id)
    {
        var success = await _harvestService.DeleteHarvestPlanAsync(id);
        
        return Ok(success 
            ? new ApiOkResponse(null, "Harvest plan deleted successfully") 
            : new ApiResponse((int)HttpStatusCode.NotFound, "Harvest plan not found"));
    }

    #endregion

    #region Harvest Schedules

    [HttpGet(Name = nameof(GetHarvestSchedules))]
    public async Task<IActionResult> GetHarvestSchedules()
    {
        var harvestSchedules = await _harvestService.GetAllHarvestSchedulesAsync();
        return Ok(harvestSchedules);
    }

    [HttpGet("{planId}", Name = nameof(GetHarvestSchedulesByPlan))]
    public async Task<IActionResult> GetHarvestSchedulesByPlan(string planId)
    {
        var harvestSchedules = await _harvestService.GetHarvestSchedulesByPlanAsync(planId);
        return Ok(harvestSchedules);
    }

    [HttpGet("{farmId}", Name = nameof(GetHarvestSchedulesByFarm))]
    public async Task<IActionResult> GetHarvestSchedulesByFarm(string farmId)
    {
        var harvestSchedules = await _harvestService.GetHarvestSchedulesByFarmAsync(farmId);
        return Ok(harvestSchedules);
    }

    [HttpGet(Name = nameof(GetUpcomingHarvests))]
    public async Task<IActionResult> GetUpcomingHarvests(string? farmId = null, int days = 30)
    {
        var upcomingHarvests = await _harvestService.GetUpcomingHarvestsAsync(farmId, days);
        return Ok(upcomingHarvests);
    }

    [HttpGet(Name = nameof(GetOverdueHarvests))]
    public async Task<IActionResult> GetOverdueHarvests(string? farmId = null)
    {
        var overdueHarvests = await _harvestService.GetOverdueHarvestsAsync(farmId);
        return Ok(overdueHarvests);
    }

    [HttpGet("{id}", Name = nameof(GetHarvestScheduleDetails))]
    public async Task<IActionResult> GetHarvestScheduleDetails(string id)
    {
        var harvestSchedule = await _harvestService.GetHarvestScheduleByIdAsync(id);
        
        return Ok(harvestSchedule == null 
            ? new ApiResponse((int)HttpStatusCode.NotFound, "Harvest schedule not found") 
            : new ApiOkResponse(harvestSchedule));
    }

    [HttpPost(Name = nameof(CreateHarvestSchedule))]
    public async Task<IActionResult> CreateHarvestSchedule([FromBody] CreateHarvestScheduleViewModel model)
    {
        try
        {
            var harvestSchedule = await _harvestService.CreateHarvestScheduleAsync(model);
            return Ok(new ApiOkResponse(harvestSchedule, "Harvest schedule created successfully"));
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, $"Error creating harvest schedule: {ex.Message}"));
        }
    }

    [HttpPut("{id}", Name = nameof(UpdateHarvestSchedule))]
    public async Task<IActionResult> UpdateHarvestSchedule(string id, [FromBody] HarvestScheduleViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        try
        {
            var harvestSchedule = await _harvestService.UpdateHarvestScheduleAsync(id, model);
            
            return Ok(harvestSchedule == null 
                ? new ApiResponse((int)HttpStatusCode.NotFound, "Harvest schedule not found") 
                : new ApiOkResponse(harvestSchedule, "Harvest schedule updated successfully"));
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, $"Error updating harvest schedule: {ex.Message}"));
        }
    }

    [HttpDelete("{id}", Name = nameof(DeleteHarvestSchedule))]
    public async Task<IActionResult> DeleteHarvestSchedule(string id)
    {
        var success = await _harvestService.DeleteHarvestScheduleAsync(id);
        
        return Ok(success 
            ? new ApiOkResponse(null, "Harvest schedule deleted successfully") 
            : new ApiResponse((int)HttpStatusCode.NotFound, "Harvest schedule not found"));
    }

    #endregion

    #region Harvest Records

    [HttpGet(Name = nameof(GetHarvestRecords))]
    public async Task<IActionResult> GetHarvestRecords()
    {
        var harvestRecords = await _harvestService.GetAllHarvestRecordsAsync();
        return Ok(harvestRecords);
    }

    [HttpGet("{scheduleId}", Name = nameof(GetHarvestRecordsBySchedule))]
    public async Task<IActionResult> GetHarvestRecordsBySchedule(string scheduleId)
    {
        var harvestRecords = await _harvestService.GetHarvestRecordsByScheduleAsync(scheduleId);
        return Ok(harvestRecords);
    }

    [HttpGet("{farmId}", Name = nameof(GetHarvestRecordsByFarm))]
    public async Task<IActionResult> GetHarvestRecordsByFarm(string farmId)
    {
        var harvestRecords = await _harvestService.GetHarvestRecordsByFarmAsync(farmId);
        return Ok(harvestRecords);
    }

    [HttpGet("{id}", Name = nameof(GetHarvestRecordDetails))]
    public async Task<IActionResult> GetHarvestRecordDetails(string id)
    {
        var harvestRecord = await _harvestService.GetHarvestRecordByIdAsync(id);
        
        return Ok(harvestRecord == null 
            ? new ApiResponse((int)HttpStatusCode.NotFound, "Harvest record not found") 
            : new ApiOkResponse(harvestRecord));
    }

    [HttpPost(Name = nameof(CreateHarvestRecord))]
    public async Task<IActionResult> CreateHarvestRecord([FromBody] CreateHarvestRecordViewModel model)
    {
        try
        {
            var harvestRecord = await _harvestService.CreateHarvestRecordAsync(model);
            return Ok(new ApiOkResponse(harvestRecord, "Harvest record created successfully"));
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, $"Error creating harvest record: {ex.Message}"));
        }
    }

    [HttpPut("{id}", Name = nameof(UpdateHarvestRecord))]
    public async Task<IActionResult> UpdateHarvestRecord(string id, [FromBody] HarvestRecordViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        try
        {
            var harvestRecord = await _harvestService.UpdateHarvestRecordAsync(id, model);
            
            return Ok(harvestRecord == null 
                ? new ApiResponse((int)HttpStatusCode.NotFound, "Harvest record not found") 
                : new ApiOkResponse(harvestRecord, "Harvest record updated successfully"));
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, $"Error updating harvest record: {ex.Message}"));
        }
    }

    [HttpDelete("{id}", Name = nameof(DeleteHarvestRecord))]
    public async Task<IActionResult> DeleteHarvestRecord(string id)
    {
        var success = await _harvestService.DeleteHarvestRecordAsync(id);
        
        return Ok(success 
            ? new ApiOkResponse(null, "Harvest record deleted successfully") 
            : new ApiResponse((int)HttpStatusCode.NotFound, "Harvest record not found"));
    }

    #endregion

    #region Harvest Losses

    [HttpGet("{recordId}", Name = nameof(GetHarvestLossesByRecord))]
    public async Task<IActionResult> GetHarvestLossesByRecord(string recordId)
    {
        var harvestLosses = await _harvestService.GetHarvestLossesByRecordAsync(recordId);
        return Ok(harvestLosses);
    }

    [HttpGet("{farmId}", Name = nameof(GetHarvestLossesByFarm))]
    public async Task<IActionResult> GetHarvestLossesByFarm(string farmId)
    {
        var harvestLosses = await _harvestService.GetHarvestLossesByFarmAsync(farmId);
        return Ok(harvestLosses);
    }

    [HttpPost(Name = nameof(CreateHarvestLoss))]
    public async Task<IActionResult> CreateHarvestLoss([FromBody] CreateHarvestLossViewModel model)
    {
        try
        {
            var harvestLoss = await _harvestService.CreateHarvestLossAsync(model);
            return Ok(new ApiOkResponse(harvestLoss, "Harvest loss recorded successfully"));
        }
        catch (Exception ex)
        {
            return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, $"Error recording harvest loss: {ex.Message}"));
        }
    }

    [HttpDelete("{id}", Name = nameof(DeleteHarvestLoss))]
    public async Task<IActionResult> DeleteHarvestLoss(string id)
    {
        var success = await _harvestService.DeleteHarvestLossAsync(id);
        
        return Ok(success 
            ? new ApiOkResponse(null, "Harvest loss deleted successfully") 
            : new ApiResponse((int)HttpStatusCode.NotFound, "Harvest loss not found"));
    }

    #endregion

    #region Analytics and Reports

    [HttpGet("{farmId}", Name = nameof(GetHarvestAnalytics))]
    public async Task<IActionResult> GetHarvestAnalytics(string farmId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var analytics = await _harvestService.GetHarvestAnalyticsAsync(farmId, startDate, endDate);
        return Ok(new ApiOkResponse(analytics));
    }

    [HttpGet("{farmId}", Name = nameof(GetHarvestHistory))]
    public async Task<IActionResult> GetHarvestHistory(string farmId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var history = await _harvestService.GetHarvestHistoryAsync(farmId, startDate, endDate);
        return Ok(history);
    }

    [HttpGet("{farmId}", Name = nameof(GetYieldComparison))]
    public async Task<IActionResult> GetYieldComparison(string farmId, string? cropId = null)
    {
        var comparison = await _harvestService.GetYieldComparisonAsync(farmId, cropId);
        return Ok(new ApiOkResponse(comparison));
    }

    #endregion
}