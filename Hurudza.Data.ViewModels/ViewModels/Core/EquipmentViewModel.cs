using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class EquipmentViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Farm is required")]
    public string FarmId { get; set; }
    
    public string? FarmName { get; set; }
    
    [Required(ErrorMessage = "Equipment type is required")]
    public EquipmentType Type { get; set; }
    
    public string? Brand { get; set; }
    
    public string? Model { get; set; }
    
    public string? SerialNumber { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? PurchaseDate { get; set; }
    
    [DataType(DataType.Currency)]
    public decimal? PurchasePrice { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? WarrantyExpiry { get; set; }
    
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Active;
    
    public EquipmentCondition Condition { get; set; } = EquipmentCondition.Good;
    
    public string? Description { get; set; }
    
    public string? Specifications { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Operating hours must be a positive number")]
    public int? OperatingHours { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? LastMaintenanceDate { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? NextMaintenanceDate { get; set; }
    
    public string? MaintenanceNotes { get; set; }
    
    public string? Location { get; set; }
    
    public string? Notes { get; set; }
    
    public List<EquipmentMaintenanceViewModel> MaintenanceRecords { get; set; } = new List<EquipmentMaintenanceViewModel>();
}

public class CreateEquipmentViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Farm is required")]
    public string FarmId { get; set; }
    
    [Required(ErrorMessage = "Equipment type is required")]
    public EquipmentType Type { get; set; }
    
    public string? Brand { get; set; }
    
    public string? Model { get; set; }
    
    public string? SerialNumber { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? PurchaseDate { get; set; }
    
    [DataType(DataType.Currency)]
    public decimal? PurchasePrice { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? WarrantyExpiry { get; set; }
    
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Active;
    
    public EquipmentCondition Condition { get; set; } = EquipmentCondition.Good;
    
    public string? Description { get; set; }
    
    public string? Specifications { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Operating hours must be a positive number")]
    public int? OperatingHours { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? LastMaintenanceDate { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? NextMaintenanceDate { get; set; }
    
    public string? MaintenanceNotes { get; set; }
    
    public string? Location { get; set; }
    
    public string? Notes { get; set; }
}

public class EquipmentListViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FarmName { get; set; }
    public EquipmentType Type { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public EquipmentStatus Status { get; set; }
    public EquipmentCondition Condition { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public int? OperatingHours { get; set; }
}