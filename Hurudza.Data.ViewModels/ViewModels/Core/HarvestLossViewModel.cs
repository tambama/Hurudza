using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class HarvestLossViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Harvest Record is required")]
    public string? HarvestRecordId { get; set; }
    
    public HarvestLossType LossType { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }
    
    public YieldUnit QuantityUnit { get; set; }
    
    [Required(ErrorMessage = "Cause is required")]
    public string? Cause { get; set; }
    
    public string? PreventionNotes { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Estimated value must be non-negative")]
    public decimal? EstimatedValue { get; set; }
    
    // Navigation properties
    public string? Field { get; set; }
    public string? Crop { get; set; }
    public decimal TotalYield { get; set; }
    
    // Computed properties
    public decimal LossPercentage => TotalYield > 0 ? (Quantity / TotalYield) * 100 : 0;
}

public class CreateHarvestLossViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Harvest Record is required")]
    public string? HarvestRecordId { get; set; }
    
    public HarvestLossType LossType { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }
    
    public YieldUnit QuantityUnit { get; set; }
    
    [Required(ErrorMessage = "Cause is required")]
    public string? Cause { get; set; }
    
    public string? PreventionNotes { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Estimated value must be non-negative")]
    public decimal? EstimatedValue { get; set; }
}