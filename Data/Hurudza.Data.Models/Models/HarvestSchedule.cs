using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class HarvestSchedule : BaseEntity
{
    public required string HarvestPlanId { get; set; }
    public required string FieldCropId { get; set; }
    public DateTime PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public decimal EstimatedYield { get; set; }
    public YieldUnit EstimatedYieldUnit { get; set; }
    public HarvestPriority Priority { get; set; }
    public HarvestStatus Status { get; set; }
    public string? EquipmentRequirements { get; set; }
    public int LaborRequirements { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual HarvestPlan HarvestPlan { get; set; }
    public virtual FieldCrop FieldCrop { get; set; }
    public virtual ICollection<HarvestRecord>? HarvestRecords { get; set; }
}