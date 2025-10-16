using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class InventoryViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Inventory type is required")]
    public InventoryType Type { get; set; }
    
    [Required(ErrorMessage = "Farm is required")]
    public string FarmId { get; set; }
    
    public string? FarmName { get; set; }
    
    public string? SKU { get; set; }
    
    public string? Barcode { get; set; }
    
    public string? Brand { get; set; }
    
    public string? Supplier { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be positive")]
    public decimal QuantityInStock { get; set; }
    
    [Required(ErrorMessage = "Unit of measure is required")]
    public UnitOfMeasure UnitOfMeasure { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Minimum stock level must be positive")]
    public decimal? MinimumStockLevel { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Reorder level must be positive")]
    public decimal? ReorderLevel { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Reorder quantity must be positive")]
    public decimal? ReorderQuantity { get; set; }
    
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Unit cost must be positive")]
    public decimal? UnitCost { get; set; }
    
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Selling price must be positive")]
    public decimal? SellingPrice { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? ExpiryDate { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? ManufactureDate { get; set; }
    
    public string? BatchNumber { get; set; }
    
    public string? StorageLocation { get; set; }
    
    public string? StorageConditions { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsPerishable { get; set; }
    
    public bool RequiresSpecialHandling { get; set; }
    
    public string? SpecialHandlingInstructions { get; set; }
    
    public DateTime? LastRestockDate { get; set; }
    
    public DateTime? LastUsedDate { get; set; }
    
    public decimal TotalValue => QuantityInStock * (UnitCost ?? 0);
    
    public bool IsLowStock => MinimumStockLevel.HasValue && QuantityInStock <= MinimumStockLevel.Value;
    
    public bool NeedsReorder => ReorderLevel.HasValue && QuantityInStock <= ReorderLevel.Value;
    
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now;
    
    public bool IsNearExpiry => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.Now.AddDays(30) && !IsExpired;
    
    public List<InventoryTransactionViewModel> Transactions { get; set; } = new List<InventoryTransactionViewModel>();
}

public class CreateInventoryViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Inventory type is required")]
    public InventoryType Type { get; set; }
    
    [Required(ErrorMessage = "Farm is required")]
    public string FarmId { get; set; }
    
    public string? SKU { get; set; }
    
    public string? Barcode { get; set; }
    
    public string? Brand { get; set; }
    
    public string? Supplier { get; set; }
    
    [Required(ErrorMessage = "Initial quantity is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be positive")]
    public decimal InitialQuantity { get; set; }
    
    [Required(ErrorMessage = "Unit of measure is required")]
    public UnitOfMeasure UnitOfMeasure { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Minimum stock level must be positive")]
    public decimal? MinimumStockLevel { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Reorder level must be positive")]
    public decimal? ReorderLevel { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Reorder quantity must be positive")]
    public decimal? ReorderQuantity { get; set; }
    
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Unit cost must be positive")]
    public decimal? UnitCost { get; set; }
    
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Selling price must be positive")]
    public decimal? SellingPrice { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? ExpiryDate { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? ManufactureDate { get; set; }
    
    public string? BatchNumber { get; set; }
    
    public string? StorageLocation { get; set; }
    
    public string? StorageConditions { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsPerishable { get; set; }
    
    public bool RequiresSpecialHandling { get; set; }
    
    public string? SpecialHandlingInstructions { get; set; }
}

public class InventoryListViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FarmName { get; set; }
    public InventoryType Type { get; set; }
    public string? SKU { get; set; }
    public string? Brand { get; set; }
    public decimal QuantityInStock { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal? MinimumStockLevel { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? StorageLocation { get; set; }
    public decimal TotalValue { get; set; }
    public bool IsLowStock { get; set; }
    public bool NeedsReorder { get; set; }
    public bool IsExpired { get; set; }
    public bool IsNearExpiry { get; set; }
}

public class InventoryAlertViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FarmName { get; set; }
    public InventoryType Type { get; set; }
    public string AlertType { get; set; } // "Low Stock", "Reorder", "Expired", "Near Expiry"
    public string AlertMessage { get; set; }
    public decimal QuantityInStock { get; set; }
    public decimal? MinimumStockLevel { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Priority { get; set; } // 1=High, 2=Medium, 3=Low
}