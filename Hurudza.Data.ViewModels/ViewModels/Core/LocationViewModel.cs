using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class LocationViewModel : BaseViewModel
{
    public string? Label { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }
}

public class FieldLocationViewModel : LocationViewModel
{
    public string? FieldId { get; set; }
    public string Field { get; set; }
}

public class FarmLocationViewModel : LocationViewModel
{
    public string? FarmId { get; set; }
    public string Farm { get; set; }
}