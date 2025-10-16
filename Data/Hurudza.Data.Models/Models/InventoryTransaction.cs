using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class InventoryTransaction : BaseEntity
{
    [Required]
    public string InventoryId { get; set; }
    public virtual Inventory Inventory { get; set; }
    
    [Required]
    public TransactionType TransactionType { get; set; }
    
    [Required]
    public decimal Quantity { get; set; }
    
    public decimal? UnitCost { get; set; }
    
    public decimal? TotalCost { get; set; }
    
    [Required]
    public DateTime TransactionDate { get; set; }
    
    [MaxLength(500)]
    public string? Reason { get; set; }
    
    [MaxLength(200)]
    public string? ReferenceNumber { get; set; }
    
    [MaxLength(200)]
    public string? SupplierInvoiceNumber { get; set; }
    
    [MaxLength(200)]
    public string? ReceivedBy { get; set; }
    
    [MaxLength(200)]
    public string? ApprovedBy { get; set; }
    
    public string? FieldId { get; set; }
    public virtual Field? Field { get; set; }
    
    public string? FieldCropId { get; set; }
    public virtual FieldCrop? FieldCrop { get; set; }
    
    [MaxLength(100)]
    public string? BatchNumber { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    public decimal QuantityBefore { get; set; }
    
    public decimal QuantityAfter { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
}