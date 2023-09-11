using Hurudza.Data.Enums.Enums;
using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models;

public class Field : DatasyncClientData, IEquatable<Field>
{
    public required string Name { get; set; }
    public string Description { get; set; }
    public SoilType SoilType { get; set; }
    public bool Irrigation { get; set; }
    public float Size { get; set; }
    public required string FarmId { get; set; }

    public bool Equals(Field other) =>
        other != null && other.Id == Id &&
        other.Name == Name &&
        other.FarmId == FarmId;
}