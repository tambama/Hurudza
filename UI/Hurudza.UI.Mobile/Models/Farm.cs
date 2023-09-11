using Hurudza.Data.Enums.Enums;
using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models;

public class Farm : DatasyncClientData, IEquatable<Farm>
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string ContactPerson { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public float Size { get; set; }
    public float Arable { get; set; }
    public float Cleared { get; set; }
    public string WaterSource { get; set; }
    public bool Irrigated { get; set; }
    public string Equipment { get; set; }
    public string SoilType { get; set; }
    public string Problems { get; set; }
    public string Personnel { get; set; }
    public string Recommendations { get; set; }
    public string WardId { get; set; }
    public string LocalAuthorityId { get; set; }
    public string DistrictId { get; set; }
    public string ProvinceId { get; set; }
    public Enums.Enums.Region Region { get; set; }
    public Conference Conference { get; set; }

    public bool Equals(Farm other) =>
        other != null && other.Id == Id &&
        other.Name == Name &&
        other.PhoneNumber == PhoneNumber &&
        other.Email == Email &&
        other.WardId == WardId &&
        other.LocalAuthorityId == LocalAuthorityId &&
        other.DistrictId == DistrictId &&
        other.ProvinceId == ProvinceId &&
        other.Region == Region &&
        other.Conference == Conference;
}