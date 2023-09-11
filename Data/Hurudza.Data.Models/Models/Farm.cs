using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Farm : BaseEntity
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string ContactPerson { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public float Size { get; set; }
    public float Arable { get; set; }
    public float Cleared { get; set; }
    public string? WaterSource { get; set; }
    public bool Irrigated { get; set; }
    public string? Equipment { get; set; }
    public string? SoilType { get; set; }
    public string? Problems { get; set; }
    public string? Personnel { get; set; }
    public string? Recommendations { get; set; }
    public string? WardId { get; set; }
    public string? LocalAuthorityId { get; set; }
    public string? DistrictId { get; set; }
    public string? ProvinceId { get; set; }
    public Region Region { get; set; }
    public Conference Conference { get; set; }

    public virtual Ward? Ward { get; set; }
    public virtual LocalAuthority? LocalAuthority { get; set; }
    public virtual District? District { get; set; }
    public virtual Province? Province { get; set; }
    public virtual ICollection<Field> Fields { get; set; }
    public virtual ICollection<FarmLocation> Locations { get; set; }
}