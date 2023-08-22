using CommunityToolkit.Mvvm.ComponentModel;

namespace Hurudza.UI.Mobile.Models;

public partial class Position : ObservableObject
{
    [ObservableProperty]
    Location _location;

    public string Address { get; }
    public string Description { get; }

    public Position(string address, string description, Location location)
    {
        Address = address;
        Description = description;
        Location = location;
    }
}