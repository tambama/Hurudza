using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class HarvestLossTypeModel
{
    public HarvestLossType Value { get; set; }
    public string Text => Value.ToString("G").Replace("_", " ");
    
    public HarvestLossTypeModel(HarvestLossType value)
    {
        Value = value;
    }
    
    public static List<HarvestLossTypeModel> GetAll()
    {
        return Enum.GetValues<HarvestLossType>()
            .Select(x => new HarvestLossTypeModel(x))
            .ToList();
    }
}