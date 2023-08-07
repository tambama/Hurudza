using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Models.Enums;
using Hurudza.Data.Models.Models;

namespace Hurudza.Data.Models.ViewModels.Core;

public class FieldViewModel
{
    public string? Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public SoilType SoilType { get; set; }
    public bool Irrigation { get; set; }
    public float Size { get; set; }
    [Required(ErrorMessage = "Farm is required")]
    public required string FarmId { get; set; }
    public string? Farm { get; set; }
    public List<FieldLocation> Locations { get; set; }
}