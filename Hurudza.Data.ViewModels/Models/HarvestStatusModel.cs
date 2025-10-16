using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class HarvestStatusModel
{
    public HarvestStatus Value { get; set; }
    public string Text => Value.ToString("G").Replace("_", " ");
    
    public HarvestStatusModel(HarvestStatus value)
    {
        Value = value;
    }
    
    public static List<HarvestStatusModel> GetAll()
    {
        return Enum.GetValues<HarvestStatus>()
            .Select(x => new HarvestStatusModel(x))
            .ToList();
    }
}