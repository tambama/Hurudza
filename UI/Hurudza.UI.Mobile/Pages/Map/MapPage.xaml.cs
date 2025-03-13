using Hurudza.UI.Mobile.Controls;
using Hurudza.UI.Mobile.Services;
using Hurudza.UI.Mobile.ViewModels.Location;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Devices.Sensors;

namespace Hurudza.UI.Mobile.Pages.Map;

public partial class MapPage : ContentPage
{
    private readonly LocationService _locationService;

    public MapPage(MapPageViewModel vm, LocationService locationService)
    {
        InitializeComponent();

        BindingContext = vm;
        _locationService = locationService;
    }

    private async void Map_OnLoaded(object sender, EventArgs e)
    {
        var location = await _locationService.GetCurrentLocation();

        var pin = new CustomPin()
        {
            Label = "Office",
            Location = new Location(location.Latitude, location.Longitude),
            Address = "Newland",
            ImageSource = ImageSource.FromUri(new Uri("https://www.gamesatlas.com/images/football/teams/ukraine/dynamo-kyiv.png"))
        };

        map.Pins.Add(pin);

        var mapSpan = new MapSpan(location, 0.01, 0.01);
        
        map.MoveToRegion(mapSpan);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new CreateFieldPage());
    }
}