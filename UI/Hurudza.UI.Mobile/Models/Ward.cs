using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models;

public class Ward : DatasyncClientData, IEquatable<Ward>
{
    public required string Name { get; set; }
    public string LocalAuthorityId { get; set; }
    public string DistrictId { get; set; }
    public string ProvinceId { get; set; }

    public virtual ICollection<Farm> Farms { get; set; }

    public bool Equals(Ward other) =>
        other != null && other.Id == Id &&
        other.Name == Name &&
        other.LocalAuthorityId == LocalAuthorityId &&
        other.DistrictId == DistrictId &&
        other.ProvinceId == ProvinceId;
}