using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class FarmTypeModel
{
    public FarmTypeModel(FarmType farmType)
    {
        Value = farmType;
    }

    public FarmType Value { get; set; }

    public virtual string Text => Value.ToString("G");
}