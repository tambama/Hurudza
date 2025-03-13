using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class RoadAccessModel
{
    public RoadAccessModel(RoadAccess roadAccess)
    {
        RoadAccess = roadAccess;
    }

    public RoadAccess RoadAccess { get; set; }

    public virtual string Name => RoadAccess.ToString("G");
}