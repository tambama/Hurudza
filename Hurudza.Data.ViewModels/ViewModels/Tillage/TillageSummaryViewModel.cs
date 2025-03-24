namespace Hurudza.Data.UI.Models.ViewModels.Tillage;

public class TillageSummaryViewModel
{
    public string FarmId { get; set; }
    public string FarmName { get; set; }
    public float TotalPlanned { get; set; }
    public float TotalTilled { get; set; }
    public float CompletionPercentage => TotalPlanned > 0 
        ? (TotalTilled / TotalPlanned) * 100 
        : 0;
    public int TotalServices { get; set; }
    public int CompletedServices { get; set; }
    public decimal TotalRevenue { get; set; }
}