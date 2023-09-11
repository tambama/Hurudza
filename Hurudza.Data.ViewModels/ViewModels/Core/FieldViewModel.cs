using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class FieldViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public SoilType SoilType { get; set; }
    public bool Irrigation { get; set; }
    public float Size { get; set; }
    [Required(ErrorMessage = "Farm is required")]
    public required string FarmId { get; set; }
    public string? Farm { get; set; }
    public List<FieldLocationViewModel> Locations { get; set; }
}