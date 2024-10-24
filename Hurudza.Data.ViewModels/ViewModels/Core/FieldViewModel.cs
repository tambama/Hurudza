using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class FieldViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public SoilType SoilType { get; set; }
    public bool Irrigation { get; set; }
    public float Size { get; set; }
    [Required(ErrorMessage = "Farm is required")]
    public string? FarmId { get; set; }
    public string? Farm { get; set; }
    public List<FieldLocationViewModel> Locations { get; set; } = new List<FieldLocationViewModel>();

    public List<List<List<double>>> Coordinates { get
        {
            Id = Id.Replace("-", string.Empty);
            var locations = Locations.OrderBy(l => l.CreatedDate).Select(l => new List<double> { l.Longitude, l.Latitude }).ToList();

            if (locations != null || locations.Count == 0)
            {
                return new List<List<List<double>>> {  locations };
            }
            else
            {
                return new List<List<List<double>>>();
            }
        }
    }
}

public class CreateFieldViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public SoilType SoilType { get; set; }
    public bool Irrigation { get; set; }
    public float Size { get; set; }
    [Required(ErrorMessage = "Farm is required")]
    public string? FarmId { get; set; }
    public string? Farm { get; set; }
}