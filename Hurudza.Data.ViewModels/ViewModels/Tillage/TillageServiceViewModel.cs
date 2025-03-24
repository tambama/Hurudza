using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Tillage;

public class TillageServiceViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Tillage program is required")]
    public string? TillageProgramId { get; set; }
    
    public string? TillageProgram { get; set; }
    
    [Required(ErrorMessage = "Recipient farm is required")]
    public string? RecipientFarmId { get; set; }
    
    public string? RecipientFarm { get; set; }
    
    [Required(ErrorMessage = "Service date is required")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime ServiceDate { get; set; }
    
    [Required(ErrorMessage = "Hectares is required")]
    [Range(0.1, float.MaxValue, ErrorMessage = "Hectares must be greater than 0")]
    public float Hectares { get; set; }
    
    [Required(ErrorMessage = "Tillage type is required")]
    public TillageType TillageType { get; set; }
    
    public string? FieldId { get; set; }
    
    public string? Field { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public decimal? ServiceCost { get; set; }
    
    // Helper properties for UI
    public string Status => IsCompleted ? "Completed" : "Scheduled";
    
    public string TillageTypeName => TillageType.ToString();
}