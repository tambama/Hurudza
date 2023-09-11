using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class LocalAuthority : BaseEntity
{
    public required string Name { get; set; }
    public string? DistrictId { get; set; }
    
    public virtual District? District { get; set; }
    public virtual ICollection<Ward> Wards { get; set; }
    public virtual ICollection<Farm>? Farms { get; set; }
}