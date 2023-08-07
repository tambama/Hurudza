using Hurudza.Data.Models.Base;
using Hurudza.Data.Models.Enums;

namespace Hurudza.Data.Models.Models;

public class Farm : BaseEntity
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public float Size { get; set; }
    public int? WardId { get; set; }
    public int? LocalAuthorityId { get; set; }
    public int? DistrictId { get; set; }
    public int? ProvinceId { get; set; }
    public Region Region { get; set; }

    public virtual Ward? Ward { get; set; }
    public virtual LocalAuthority? LocalAuthority { get; set; }
    public virtual District? District { get; set; }
    public virtual Province? Province { get; set; }
    public virtual ICollection<Field> Fields { get; set; }
    public virtual ICollection<FarmLocation> Locations { get; set; }
}