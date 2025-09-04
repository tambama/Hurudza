using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class EquipmentMaintenance : BaseEntity
{
    [Required]
    public string EquipmentId { get; set; }
    public virtual Equipment Equipment { get; set; }
    
    [Required]
    public DateTime MaintenanceDate { get; set; }
    
    [Required]
    public string MaintenanceType { get; set; }  // Routine, Repair, Inspection, etc.
    
    public string? Description { get; set; }
    
    public string? PartsReplaced { get; set; }
    
    public decimal? Cost { get; set; }
    
    public string? ServiceProvider { get; set; }
    
    public string? TechnicianName { get; set; }
    
    public int? OperatingHoursAtMaintenance { get; set; }
    
    public DateTime? NextScheduledMaintenance { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsCompleted { get; set; } = true;
}