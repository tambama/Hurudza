using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models;

public class FieldCrop : DatasyncClientData, IEquatable<FieldCrop>
{
    public required string FieldId { get; set; }
    public required string CropId { get; set; }
    public DateTime? PlantedDate { get; set; }
    public DateTime? HarvestDate { get; set; }
    public float Size { get; set; }
    public bool Irrigation { get; set; }

    public bool Equals(FieldCrop other) =>
        other != null && other.Id == Id &&
        other.FieldId == FieldId &&
        other.CropId == CropId;
}