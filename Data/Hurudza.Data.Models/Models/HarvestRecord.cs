using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class HarvestRecord : BaseEntity
{
    public required string HarvestScheduleId { get; set; }
    public decimal ActualYield { get; set; }
    public YieldUnit YieldUnit { get; set; }
    public QualityGrade Quality { get; set; }
    public DateTime HarvestDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public decimal? LaborHours { get; set; }
    public decimal? LaborCost { get; set; }
    public string? EquipmentUsed { get; set; }
    public string? StorageLocation { get; set; }
    public decimal? Moisture { get; set; }
    public decimal? Temperature { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual HarvestSchedule HarvestSchedule { get; set; }
    public virtual ICollection<HarvestLoss>? HarvestLosses { get; set; }
}