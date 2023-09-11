using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Field : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public SoilType SoilType { get; set; }
    public bool Irrigation { get; set; }
    public float Size { get; set; }
    public required string FarmId { get; set; }
    public virtual Farm? Farm { get; set; }
    public virtual ICollection<FieldLocation>? Locations { get; set; }
    public virtual ICollection<FieldCrop>? Crops { get; set; }
}