using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models
{
    public class District : DatasyncClientData, IEquatable<District>
    {
        public required string Name { get; set; }
        public required string ProvinceId { get; set; }

        public bool Equals(District other) => other != null && other.Id == Id && other.Name == Name && other.ProvinceId == ProvinceId;
    }
}