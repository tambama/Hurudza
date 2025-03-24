using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class TillageTypeModel
{
    public TillageTypeModel(TillageType tillageType)
    {
        TillageType = tillageType;
    }

    public TillageType TillageType { get; set; }

    public virtual string Name => TillageType.ToString("G");
}