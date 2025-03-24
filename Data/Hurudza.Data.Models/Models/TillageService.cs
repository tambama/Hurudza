using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

/// <summary>
/// Represents an individual tillage service provided to a farm
/// </summary>
public class TillageService : BaseEntity
{
    public required string TillageProgramId { get; set; }
    public required string RecipientFarmId { get; set; } // The farm receiving the service
    public DateTime ServiceDate { get; set; }
    public float Hectares { get; set; }
    public TillageType TillageType { get; set; }
    public string? FieldId { get; set; } // Optional link to a specific field
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
    public decimal? ServiceCost { get; set; } // Optional cost of service
    
    // Navigation properties
    public virtual TillageProgram? TillageProgram { get; set; }
    public virtual Farm? RecipientFarm { get; set; }
    public virtual Field? Field { get; set; }
}