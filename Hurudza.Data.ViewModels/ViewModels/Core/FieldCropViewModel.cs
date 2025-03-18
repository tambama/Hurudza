using Hurudza.Data.UI.Models.ViewModels.Base;
using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class FieldCropViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Field must be selected")]
    public string FieldId { get; set; }
    [Required(ErrorMessage = "Crop must be selected")]
    public string? CropId { get; set; }
    public DateTime? PlantedDate { get; set; }
    public DateTime? HarvestDate { get; set; }
    public float Size { get; set; }
    public bool Irrigation { get; set; }
    public string? Field { get; set; }
    public string? Crop { get; set; }
}