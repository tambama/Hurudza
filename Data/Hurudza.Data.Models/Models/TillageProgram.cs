using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

/// <summary>
/// Represents a tillage program for a specific season
/// </summary>
public class TillageProgram : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string FarmId { get; set; }  // The farm that owns the tractor/equipment
    public float TotalHectares { get; set; } // Total hectares planned for the program
    public float TilledHectares { get; set; } // Hectares that have been tilled so far
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Farm? Farm { get; set; }
    public virtual ICollection<TillageService> TillageServices { get; set; }
}