using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class EquipmentConditionModel
{
    public EquipmentConditionModel(EquipmentCondition condition)
    {
        Value = condition;
    }

    public EquipmentCondition Value { get; set; }

    public virtual string Text => Value.ToString("G").Replace("_", " ");
}