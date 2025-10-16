using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class HarvestRecordViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Harvest Schedule is required")]
    public string? HarvestScheduleId { get; set; }
    
    [Required(ErrorMessage = "Actual yield is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Actual yield must be greater than 0")]
    public decimal ActualYield { get; set; }
    
    public YieldUnit YieldUnit { get; set; }
    public QualityGrade Quality { get; set; }
    
    [Required(ErrorMessage = "Harvest date is required")]
    public DateTime HarvestDate { get; set; }
    
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Labor hours must be non-negative")]
    public decimal? LaborHours { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Labor cost must be non-negative")]
    public decimal? LaborCost { get; set; }
    
    public string? EquipmentUsed { get; set; }
    public string? StorageLocation { get; set; }
    
    [Range(0, 100, ErrorMessage = "Moisture must be between 0 and 100")]
    public decimal? Moisture { get; set; }
    
    public decimal? Temperature { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public string? Field { get; set; }
    public string? Crop { get; set; }
    public string? Farm { get; set; }
    public decimal EstimatedYield { get; set; }
    public List<HarvestLossViewModel> HarvestLosses { get; set; } = new List<HarvestLossViewModel>();
    
    // Computed properties
    public decimal YieldVariance => ActualYield - EstimatedYield;
    public decimal YieldVariancePercentage => EstimatedYield > 0 ? (YieldVariance / EstimatedYield) * 100 : 0;
    public decimal TotalLoss => HarvestLosses.Sum(l => l.Quantity);
    public TimeSpan? Duration => StartTime.HasValue && EndTime.HasValue ? EndTime - StartTime : null;
    public decimal? HourlyYield => Duration?.TotalHours > 0 ? ActualYield / (decimal)Duration.Value.TotalHours : null;
}

public class CreateHarvestRecordViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Harvest Schedule is required")]
    public string? HarvestScheduleId { get; set; }
    
    [Required(ErrorMessage = "Actual yield is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Actual yield must be greater than 0")]
    public decimal ActualYield { get; set; }
    
    public YieldUnit YieldUnit { get; set; }
    public QualityGrade Quality { get; set; }
    
    [Required(ErrorMessage = "Harvest date is required")]
    public DateTime HarvestDate { get; set; }
    
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Labor hours must be non-negative")]
    public decimal? LaborHours { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Labor cost must be non-negative")]
    public decimal? LaborCost { get; set; }
    
    public string? EquipmentUsed { get; set; }
    public string? StorageLocation { get; set; }
    
    [Range(0, 100, ErrorMessage = "Moisture must be between 0 and 100")]
    public decimal? Moisture { get; set; }

    public decimal? Temperature { get; set; }
    public string? Notes { get; set; }

    /// <summary>
    /// Indicates whether this harvest completes the scheduled harvest.
    /// If false, the harvest schedule status will be set to InProgress, allowing for additional partial harvests.
    /// If true, the harvest schedule status will be set to Completed.
    /// </summary>
    public bool IsCompleteHarvest { get; set; } = true;
}