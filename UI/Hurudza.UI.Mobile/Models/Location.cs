using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models;

public class Location : DatasyncClientData, IEquatable<Location>
{
    public string Label { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }

    public bool Equals(Location other) =>
        other != null && other.Id == Id &&
        other.Label == Label &&
        other.Longitude == Longitude &&
        other.Latitude == Latitude;
}

public class FieldLocation : Location
{
    public string FieldId { get; set; }
    public virtual Field Field { get; set; }
}

public class FarmLocation : Location
{
    public string FarmId { get; set; }
    public virtual Farm Farm { get; set; }
}