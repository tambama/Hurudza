using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models;

public class LocalAuthority : DatasyncClientData, IEquatable<LocalAuthority>
{
    public required string Name { get; set; }
    public string DistrictId { get; set; }

    public bool Equals(LocalAuthority other) =>
        other != null && other.Id == Id &&
        other.Name == Name &&
        other.DistrictId == DistrictId;
}