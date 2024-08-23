using Hurudza.UI.Mobile.Controls;
using Hurudza.UI.Mobile.Pages.Fields;
using Hurudza.UI.Mobile.Services;
using Hurudza.UI.Mobile.ViewModels.Location;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.Specialized;
using The49.Maui.BottomSheet;
using ServiceProvider = Hurudza.UI.Mobile.Extensions.ServiceProvider;
using Microsoft.Maui.Devices.Sensors;

namespace Hurudza.UI.Mobile.Pages.Map;

public partial class CreateFieldPage : ContentPage
{
    private readonly LocationService _locationService;
    private readonly CreateFieldOnMapViewModel _viewModel;
    private readonly CreateFieldSheet _createFieldSheet;
    private readonly MapSpan _mapSpan;

    public CreateFieldPage()
	{
		InitializeComponent();

        Shell.SetNavBarIsVisible(this, false);

        _locationService = ServiceProvider.GetService<LocationService>();
        _viewModel = ServiceProvider.GetService<CreateFieldOnMapViewModel>();
        _viewModel.InitCommand.Execute(null);
        BindingContext = _viewModel;

        _createFieldSheet = new CreateFieldSheet();
        _createFieldSheet.BindingContext = _viewModel;
        _createFieldSheet.Detents = new DetentsCollection()
        {
            new RatioDetent(){ Ratio = 0.93F },
            new AnchorDetent(){ Anchor = _createFieldSheet.Divider },
            new ContentDetent(),
        };


        _viewModel.CreateFieldSheet = _createFieldSheet;


        _createFieldSheet.ShowAsync();

        _viewModel.Boundaries.CollectionChanged += Boundaries_CollectionChanged;
    }

    private void Boundaries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        map.MapElements.Clear();
        map.Pins.Clear();

        if (_viewModel.LabelPoint != null)
        {
            Polygon polygon = new()
            {
                StrokeWidth = 8,
                StrokeColor = Color.FromArgb("#1BA1E2"),
                FillColor = Colors.Transparent,
                Geopath =
                {
                    new () { Latitude = -18.770168306913888, Longitude = 32.23066517235199 },
                    new () { Latitude = -18.770444432902877, Longitude = 32.231199253347256 },
                    new () { Latitude = -18.77082368954858, Longitude = 32.23177549863135 },
                    new () { Latitude = -18.769772414647107, Longitude = 32.23212335401618 },
                    new () { Latitude = -18.769057144748274, Longitude = 32.23228849748119 },
                    new () { Latitude = -18.768844226610582, Longitude = 32.23137493788491 },
                    new () { Latitude = -18.770168306913888, Longitude = 32.23066517235199 },
                }
            };

            map.MapElements.Add(polygon);

            var pin = new CustomPin()
            {
                Label = "Block A",
                Location = _viewModel.LabelPoint,
                Address = _viewModel.Area,
                ImageSource = ImageSource.FromUri(new Uri("https://www.gamesatlas.com/images/football/teams/ukraine/dynamo-kyiv.png"))
                //ImageSource = ImageSource.FromResource("Hurudza.UI.Mobile.Resources.Images.information.png")
            };

            map.Pins.Add(pin);

            map.MapType = MapType.Hybrid;
            var mapSpan = new MapSpan(_viewModel.LabelPoint, 0.01, 0.01);
            map.MoveToRegion(mapSpan);
        }
    }

    private async void Map_OnLoaded(object sender, EventArgs e)
    {
        var location = await _locationService.GetCurrentLocation();

        var mapSpan = new MapSpan(location, 0.01, 0.01);

        map.MoveToRegion(mapSpan);
    }

    private async void Cancel_Pressed(object sender, EventArgs e)
    {
        await _createFieldSheet.DismissAsync();
        await Shell.Current.Navigation.PopAsync();
    }
}