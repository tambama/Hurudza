using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class HarvestPriorityModel
{
    public HarvestPriority Value { get; set; }
    public string Text => Value.ToString("G").Replace("_", " ");
    
    public HarvestPriorityModel(HarvestPriority value)
    {
        Value = value;
    }
    
    public static List<HarvestPriorityModel> GetAll()
    {
        return Enum.GetValues<HarvestPriority>()
            .Select(x => new HarvestPriorityModel(x))
            .ToList();
    }
}