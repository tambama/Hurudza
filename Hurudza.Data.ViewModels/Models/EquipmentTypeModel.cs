using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class EquipmentTypeModel
{
    public EquipmentTypeModel(EquipmentType equipmentType)
    {
        Value = equipmentType;
    }

    public EquipmentType Value { get; set; }

    public virtual string Text => Value.ToString("G").Replace("_", " ");
}