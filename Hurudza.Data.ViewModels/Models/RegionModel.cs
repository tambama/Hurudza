using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class RegionModel
{
    public RegionModel(Region regions)
    {
        Region = regions;
    }

    public Region Region { get; set; }

    public virtual string Name => Region.ToString("G");
}