using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class InventoryTypeModel
{
    public InventoryType Value { get; set; }
    public string Text { get; set; }

    public InventoryTypeModel()
    {
    }

    public InventoryTypeModel(InventoryType value)
    {
        Value = value;
        Text = value.ToString("G").Replace("_", " ");
    }
}