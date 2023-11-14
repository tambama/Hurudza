using Hurudza.Common.Utils.Extensions;
using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class WaterAvailabilityModel
{
    public WaterAvailabilityModel(WaterAvailability waterAvailability)
    {
        WaterAvailability = waterAvailability;
    }

    public WaterAvailability WaterAvailability { get; set; }

    public virtual string Name => WaterAvailability.GetDescription();
}