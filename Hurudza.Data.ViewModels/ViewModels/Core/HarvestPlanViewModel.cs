using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class HarvestPlanViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Farm is required")]
    public string? FarmId { get; set; }
    
    [Required(ErrorMessage = "Season is required")]
    public string? Season { get; set; }
    
    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }
    
    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }
    
    public HarvestPlanStatus Status { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public string? Farm { get; set; }
    public List<HarvestScheduleViewModel> HarvestSchedules { get; set; } = new List<HarvestScheduleViewModel>();
    
    // Computed properties
    public int TotalScheduledHarvests => HarvestSchedules.Count;
    public int CompletedHarvests => HarvestSchedules.Count(h => h.Status == HarvestStatus.Completed);
    public decimal Progress => TotalScheduledHarvests > 0 ? (decimal)CompletedHarvests / TotalScheduledHarvests * 100 : 0;
}

public class CreateHarvestPlanViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Farm is required")]
    public string? FarmId { get; set; }
    
    [Required(ErrorMessage = "Season is required")]
    public string? Season { get; set; }
    
    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }
    
    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }
    
    public string? Notes { get; set; }
}