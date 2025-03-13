using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Ward : BaseEntity
{
    public required string Name { get; set; }
    public string? LocalAuthorityId { get; set; }
    public string? DistrictId { get; set; }
    public string? ProvinceId { get; set; }
    
    public virtual LocalAuthority? LocalAuthority { get; set; }
    public virtual District? District { get; set; }
    public virtual Province? Province { get; set; }
    
    public virtual ICollection<Farm>? Farms { get; set; }
}