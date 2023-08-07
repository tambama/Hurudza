using Hurudza.Data.Models.Enums;

namespace Hurudza.Data.Models.Models;

public class SoilTypeModel
{
    public SoilTypeModel(SoilType regions)
    {
        SoilType = regions;
    }
    
    public SoilType SoilType { get; set; }
    
    public virtual string Name => SoilType.ToString("G");
}