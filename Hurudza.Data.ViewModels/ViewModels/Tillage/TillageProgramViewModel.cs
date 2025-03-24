using System.ComponentModel.DataAnnotations;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Tillage;

public class TillageProgramViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Program name is required")]
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Start date is required")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }
    
    [Required(ErrorMessage = "End date is required")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime EndDate { get; set; }
    
    [Required(ErrorMessage = "Farm is required")]
    public string? FarmId { get; set; }
    
    public string? Farm { get; set; }
    
    [Required(ErrorMessage = "Total hectares is required")]
    [Range(0.1, float.MaxValue, ErrorMessage = "Total hectares must be greater than 0")]
    public float TotalHectares { get; set; }
    
    public float TilledHectares { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Helper properties for UI
    public float RemainingHectares => TotalHectares - TilledHectares;
    
    public float CompletionPercentage => TotalHectares > 0 
        ? (TilledHectares / TotalHectares) * 100 
        : 0;
    
    public string Status => IsActive ? "Active" : "Completed";
    
    public string DateRange => $"{StartDate.ToString("MMM dd, yyyy")} - {EndDate.ToString("MMM dd, yyyy")}";
    
    public string Progress => $"{TilledHectares:N2} of {TotalHectares:N2} ha ({CompletionPercentage:N0}%)";
}