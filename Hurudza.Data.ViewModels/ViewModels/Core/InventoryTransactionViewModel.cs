using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class InventoryTransactionViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Inventory item is required")]
    public string InventoryId { get; set; }
    
    public string? InventoryName { get; set; }
    
    [Required(ErrorMessage = "Transaction type is required")]
    public TransactionType TransactionType { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public decimal Quantity { get; set; }
    
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Unit cost must be positive")]
    public decimal? UnitCost { get; set; }
    
    [DataType(DataType.Currency)]
    public decimal? TotalCost { get; set; }
    
    [Required(ErrorMessage = "Transaction date is required")]
    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; }
    
    public string? Reason { get; set; }
    
    public string? ReferenceNumber { get; set; }
    
    public string? SupplierInvoiceNumber { get; set; }
    
    public string? ReceivedBy { get; set; }
    
    public string? ApprovedBy { get; set; }
    
    public string? FieldId { get; set; }
    
    public string? FieldName { get; set; }
    
    public string? FieldCropId { get; set; }
    
    public string? CropName { get; set; }
    
    public string? BatchNumber { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? ExpiryDate { get; set; }
    
    public decimal QuantityBefore { get; set; }
    
    public decimal QuantityAfter { get; set; }
    
    public string? Notes { get; set; }
}

public class CreateInventoryTransactionViewModel
{
    [Required(ErrorMessage = "Inventory item is required")]
    public string InventoryId { get; set; }
    
    [Required(ErrorMessage = "Transaction type is required")]
    public TransactionType TransactionType { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public decimal Quantity { get; set; }
    
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Unit cost must be positive")]
    public decimal? UnitCost { get; set; }
    
    [Required(ErrorMessage = "Transaction date is required")]
    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    
    public string? Reason { get; set; }
    
    public string? ReferenceNumber { get; set; }
    
    public string? SupplierInvoiceNumber { get; set; }
    
    public string? ReceivedBy { get; set; }
    
    public string? ApprovedBy { get; set; }
    
    public string? FieldId { get; set; }
    
    public string? FieldCropId { get; set; }
    
    public string? BatchNumber { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? ExpiryDate { get; set; }
    
    public string? Notes { get; set; }
}

public class InventoryTransactionListViewModel
{
    public string Id { get; set; }
    public string InventoryName { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Quantity { get; set; }
    public decimal? TotalCost { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? FieldName { get; set; }
    public string? CropName { get; set; }
    public decimal QuantityBefore { get; set; }
    public decimal QuantityAfter { get; set; }
    public string? PerformedBy { get; set; }
}