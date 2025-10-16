using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class UnitOfMeasureModel
{
    public UnitOfMeasure Value { get; set; }
    public string Text { get; set; }

    public UnitOfMeasureModel()
    {
    }

    public UnitOfMeasureModel(UnitOfMeasure value)
    {
        Value = value;
        Text = value.ToString("G").Replace("_", " ");
    }
}