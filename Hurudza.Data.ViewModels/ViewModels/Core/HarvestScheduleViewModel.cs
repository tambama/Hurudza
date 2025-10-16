using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class HarvestScheduleViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Harvest Plan is required")]
    public string? HarvestPlanId { get; set; }

    [Required(ErrorMessage = "Field Crop is required")]
    public string? FieldCropId { get; set; }

    [Required(ErrorMessage = "Planned date is required")]
    public DateTime PlannedDate { get; set; }

    public DateTime? ActualDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? CreatedBy { get; set; }

    [Required(ErrorMessage = "Estimated yield is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Estimated yield must be greater than 0")]
    public decimal EstimatedYield { get; set; }

    public YieldUnit EstimatedYieldUnit { get; set; }
    public HarvestPriority Priority { get; set; }
    public HarvestStatus Status { get; set; }
    public string? EquipmentRequirements { get; set; }
    public int LaborRequirements { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public string? HarvestPlan { get; set; }
    public string? Field { get; set; }
    public string? Crop { get; set; }
    public string? Farm { get; set; }
    public List<HarvestRecordViewModel> HarvestRecords { get; set; } = new List<HarvestRecordViewModel>();
    
    // Computed properties
    public decimal ActualYield => HarvestRecords.Sum(r => r.ActualYield);
    public bool IsOverdue => PlannedDate < DateTime.Now && Status == HarvestStatus.Planned;
    public int DaysFromPlanned => (DateTime.Now - PlannedDate).Days;
}

public class CreateHarvestScheduleViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Harvest Plan is required")]
    public string? HarvestPlanId { get; set; }
    
    [Required(ErrorMessage = "Field Crop is required")]
    public string? FieldCropId { get; set; }
    
    [Required(ErrorMessage = "Planned date is required")]
    public DateTime PlannedDate { get; set; }
    
    [Required(ErrorMessage = "Estimated yield is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Estimated yield must be greater than 0")]
    public decimal EstimatedYield { get; set; }
    
    public YieldUnit EstimatedYieldUnit { get; set; }
    public HarvestPriority Priority { get; set; }
    public string? EquipmentRequirements { get; set; }
    public int LaborRequirements { get; set; }
    public string? Notes { get; set; }
}