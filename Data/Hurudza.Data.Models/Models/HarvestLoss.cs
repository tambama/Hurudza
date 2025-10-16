using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class HarvestLoss : BaseEntity
{
    public required string HarvestRecordId { get; set; }
    public HarvestLossType LossType { get; set; }
    public decimal Quantity { get; set; }
    public YieldUnit QuantityUnit { get; set; }
    public required string Cause { get; set; }
    public string? PreventionNotes { get; set; }
    public decimal? EstimatedValue { get; set; }
    
    // Navigation properties
    public virtual HarvestRecord HarvestRecord { get; set; }
}