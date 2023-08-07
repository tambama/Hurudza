namespace Hurudza.Data.Models.Models;

public class LocalAuthority
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? DistrictId { get; set; }
    
    public virtual District? District { get; set; }
    public virtual ICollection<Ward> Wards { get; set; }
    public virtual ICollection<Farm>? Farms { get; set; }
}