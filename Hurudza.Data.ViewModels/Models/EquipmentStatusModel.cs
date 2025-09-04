using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class EquipmentStatusModel
{
    public EquipmentStatusModel(EquipmentStatus status)
    {
        Value = status;
    }

    public EquipmentStatus Value { get; set; }

    public virtual string Text => Value.ToString("G").Replace("_", " ");
}