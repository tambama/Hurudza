using System.ComponentModel.DataAnnotations;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class EquipmentMaintenanceViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Equipment is required")]
    public string EquipmentId { get; set; }
    
    public string? EquipmentName { get; set; }
    
    [Required(ErrorMessage = "Maintenance date is required")]
    [DataType(DataType.Date)]
    public DateTime MaintenanceDate { get; set; }
    
    [Required(ErrorMessage = "Maintenance type is required")]
    public string MaintenanceType { get; set; }
    
    public string? Description { get; set; }
    
    public string? PartsReplaced { get; set; }
    
    [DataType(DataType.Currency)]
    public decimal? Cost { get; set; }
    
    public string? ServiceProvider { get; set; }
    
    public string? TechnicianName { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Operating hours must be a positive number")]
    public int? OperatingHoursAtMaintenance { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? NextScheduledMaintenance { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsCompleted { get; set; } = true;
}

public class CreateEquipmentMaintenanceViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Equipment is required")]
    public string EquipmentId { get; set; }
    
    [Required(ErrorMessage = "Maintenance date is required")]
    [DataType(DataType.Date)]
    public DateTime MaintenanceDate { get; set; } = DateTime.Today;
    
    [Required(ErrorMessage = "Maintenance type is required")]
    public string MaintenanceType { get; set; }
    
    public string? Description { get; set; }
    
    public string? PartsReplaced { get; set; }
    
    [DataType(DataType.Currency)]
    public decimal? Cost { get; set; }
    
    public string? ServiceProvider { get; set; }
    
    public string? TechnicianName { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Operating hours must be a positive number")]
    public int? OperatingHoursAtMaintenance { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? NextScheduledMaintenance { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsCompleted { get; set; } = true;
}

public class EquipmentMaintenanceListViewModel
{
    public string Id { get; set; }
    public string EquipmentName { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public string MaintenanceType { get; set; }
    public string? Description { get; set; }
    public decimal? Cost { get; set; }
    public string? ServiceProvider { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? NextScheduledMaintenance { get; set; }
}