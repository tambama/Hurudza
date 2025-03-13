using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class SoilTypeModel
{
    public SoilTypeModel(SoilType regions)
    {
        SoilType = regions;
    }

    public SoilType SoilType { get; set; }

    public virtual string Name => SoilType.ToString("G");
}
