using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Models;

public class Crop : DatasyncClientData, IEquatable<Crop>
{
    public required string Name { get; set; }

    public bool Equals(Crop other) => other != null && other.Id == Id && other.Name == Name;
}