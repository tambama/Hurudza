using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class FieldCrop : BaseEntity
{
    public required string FieldId { get; set; }
    public required string CropId { get; set; }
    public DateTime? PlantedDate { get; set; }
    public DateTime? HarvestDate { get; set; }
    public float Size { get; set; }
    public bool Irrigation { get; set; }
    
    public virtual Field Field { get; set; }
    public virtual Crop Crop { get; set; }
}