using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.Models.ViewModels.Core;

public class FieldCropViewModel
{
    public string? Id { get; set; }
    [Required(ErrorMessage = "Field must be selected")]
    public required string FieldId { get; set; }
    [Required(ErrorMessage = "Crop must be selected")]
    public int CropId { get; set; }
    public DateTime? PlantedDate { get; set; }
    public DateTime? HarvestDate { get; set; }
    public float Size { get; set; }
    public bool Irrigation { get; set; }
    public string? Field { get; set; }
    public string? Crop { get; set; }
}