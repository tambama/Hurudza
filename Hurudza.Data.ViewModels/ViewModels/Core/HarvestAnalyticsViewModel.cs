namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class HarvestAnalyticsViewModel
{
    public int TotalHarvests { get; set; }
    public decimal TotalYield { get; set; }
    public decimal AverageYield { get; set; }
    public decimal TotalLosses { get; set; }
    public List<YieldByCropViewModel> YieldByCrop { get; set; } = new();
    public List<LossByTypeViewModel> LossByType { get; set; } = new();
}

public class YieldByCropViewModel
{
    public string Crop { get; set; } = string.Empty;
    public decimal TotalYield { get; set; }
}

public class LossByTypeViewModel
{
    public string LossType { get; set; } = string.Empty;
    public decimal TotalLoss { get; set; }
}

public class YieldComparisonViewModel
{
    public List<CropYieldComparisonViewModel> Comparisons { get; set; } = new();
}

public class CropYieldComparisonViewModel
{
    public string CropId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal TotalYield { get; set; }
    public decimal AverageYield { get; set; }
    public decimal EstimatedYield { get; set; }
    public decimal YieldVariance { get; set; }
}
