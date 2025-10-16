using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class HarvestPlanStatusModel
{
    public HarvestPlanStatus Value { get; set; }
    public string Text => Value.ToString("G").Replace("_", " ");
    
    public HarvestPlanStatusModel(HarvestPlanStatus value)
    {
        Value = value;
    }
    
    public static List<HarvestPlanStatusModel> GetAll()
    {
        return Enum.GetValues<HarvestPlanStatus>()
            .Select(x => new HarvestPlanStatusModel(x))
            .ToList();
    }
}