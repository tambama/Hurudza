using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class LocationViewModel : BaseViewModel
{
    public string? Label { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public double Altitude { get; set; }

    public string Text => $"{Latitude}, {Longitude}";
}

public class FieldLocationViewModel : LocationViewModel
{
    public string? FieldId { get; set; }
    public string? Field { get; set; }
}

public class FarmLocationViewModel : LocationViewModel
{
    public string? FarmId { get; set; }
    public string? Farm { get; set; }
}