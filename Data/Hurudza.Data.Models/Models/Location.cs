using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Location : BaseEntity
{
    public string? Label { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }
}

public class FieldLocation : Location
{
    public string? FieldId { get; set; }
    public virtual Field? Field { get; set; }
}

public class FarmLocation : Location
{
    public string? FarmId { get; set; }
    public virtual Farm? Farm { get; set; }
}