using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class YieldUnitModel
{
    public YieldUnit Value { get; set; }
    public string Text => Value.ToString("G").Replace("_", " ");
    
    public YieldUnitModel(YieldUnit value)
    {
        Value = value;
    }
    
    public static List<YieldUnitModel> GetAll()
    {
        return Enum.GetValues<YieldUnit>()
            .Select(x => new YieldUnitModel(x))
            .ToList();
    }
}