namespace Hurudza.Data.UI.Models.ViewModels.Stats;

public class TillageStatisticsViewModel
{
    public int TotalPrograms { get; set; }
    public int TotalServices { get; set; }
    public int CompletedServices { get; set; }
    public float TotalPlannedHectares { get; set; }
    public float TotalTilledHectares { get; set; }
    public decimal TotalRevenue { get; set; }
}