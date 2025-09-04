using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Equipment : BaseEntity
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string FarmId { get; set; }
    public virtual Farm Farm { get; set; }
    
    [Required]
    public EquipmentType Type { get; set; }
    
    public string? Brand { get; set; }
    
    public string? Model { get; set; }
    
    public string? SerialNumber { get; set; }
    
    public DateTime? PurchaseDate { get; set; }
    
    public decimal? PurchasePrice { get; set; }
    
    public DateTime? WarrantyExpiry { get; set; }
    
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Active;
    
    public EquipmentCondition Condition { get; set; } = EquipmentCondition.Good;
    
    public string? Description { get; set; }
    
    public string? Specifications { get; set; }
    
    public int? OperatingHours { get; set; }
    
    public DateTime? LastMaintenanceDate { get; set; }
    
    public DateTime? NextMaintenanceDate { get; set; }
    
    public string? MaintenanceNotes { get; set; }
    
    public string? Location { get; set; }
    
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual ICollection<EquipmentMaintenance> MaintenanceRecords { get; set; } = new List<EquipmentMaintenance>();
}