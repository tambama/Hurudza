using CommunityToolkit.Mvvm.ComponentModel;
using Device = Microsoft.Maui.Devices.Sensors;

namespace Hurudza.UI.Mobile.Models
{
    public partial class Position : ObservableObject
    {
        [ObservableProperty]
        Device.Location _location;

        public string Address { get; }
        public string Description { get; }

        public Position(string address, string description, Device.Location location)
        {
            Address = address;
            Description = description;
            Location = location;
        }
    }
}