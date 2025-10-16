using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Inventory : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public InventoryType Type { get; set; }
    
    [Required]
    public string FarmId { get; set; }
    public virtual Farm Farm { get; set; }
    
    [MaxLength(100)]
    public string? SKU { get; set; }
    
    [MaxLength(100)]
    public string? Barcode { get; set; }
    
    [MaxLength(100)]
    public string? Brand { get; set; }
    
    [MaxLength(100)]
    public string? Supplier { get; set; }
    
    [Required]
    public decimal QuantityInStock { get; set; }
    
    [Required]
    public UnitOfMeasure UnitOfMeasure { get; set; }
    
    public decimal? MinimumStockLevel { get; set; }
    
    public decimal? ReorderLevel { get; set; }
    
    public decimal? ReorderQuantity { get; set; }
    
    public decimal? UnitCost { get; set; }
    
    public decimal? SellingPrice { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    public DateTime? ManufactureDate { get; set; }
    
    [MaxLength(100)]
    public string? BatchNumber { get; set; }
    
    [MaxLength(200)]
    public string? StorageLocation { get; set; }
    
    [MaxLength(500)]
    public string? StorageConditions { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public bool IsPerishable { get; set; }
    
    public bool RequiresSpecialHandling { get; set; }
    
    [MaxLength(500)]
    public string? SpecialHandlingInstructions { get; set; }
    
    public DateTime? LastRestockDate { get; set; }
    
    public DateTime? LastUsedDate { get; set; }
    
    public virtual ICollection<InventoryTransaction> Transactions { get; set; }
}