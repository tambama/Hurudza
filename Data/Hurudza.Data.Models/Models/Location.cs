using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Location : BaseEntity
{
    public string? Label { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public double Altitude { get; set; }
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