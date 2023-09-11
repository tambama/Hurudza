using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models;

public class Province : DatasyncClientData, IEquatable<Province>
{
    public required string Name { get; set; }

    public bool Equals(Province other) => other != null && other.Id == Id && other.Name == Name;
}