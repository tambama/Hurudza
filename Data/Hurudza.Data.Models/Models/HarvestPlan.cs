using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class HarvestPlan : BaseEntity
{
    public required string FarmId { get; set; }
    public required string Season { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public HarvestPlanStatus Status { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual Farm Farm { get; set; }
    public virtual ICollection<HarvestSchedule>? HarvestSchedules { get; set; }
}