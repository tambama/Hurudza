using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Hurudza.Common.Utils.Extensions;
using Hurudza.Common.Utils.Models;
using Hurudza.UI.Mobile.Models;
using Hurudza.UI.Mobile.Services;
using Microsoft.Maui.Maps;
using Sensors = Microsoft.Maui.Devices.Sensors;
using Maps = Microsoft.Maui.Controls.Maps;
using Hurudza.Data.Enums.Enums;

namespace Hurudza.UI.Mobile.ViewModels.Location;

public partial class CreateFieldOnMapViewModel : ObservableObject
{
    private readonly LocationService _locationService;

    public ObservableCollection<EnumItem> SoilTypes { get; set; }

    public The49.Maui.BottomSheet.BottomSheet CreateFieldSheet { get; set; }

    public CreateFieldOnMapViewModel(LocationService locationService)
    {
        _locationService = locationService;
        var enumItems = typeof(SoilType).ToList();
        SoilTypes = new ObservableCollection<EnumItem>(enumItems);
        Boundaries = new ObservableCollection<Sensors.Location>();
        Area = "0Ha";

        var timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += async (s, e) => await OnTimedEvent(s, e);
        timer.Start();
    }

    [ObservableProperty]
    ObservableCollection<Sensors.Location> _boundaries;

    [ObservableProperty]
    ObservableCollection<Maps.Polygon> _polygons;

    [ObservableProperty]
    Sensors.Location _currentLocation;

    [ObservableProperty]
    double _accuracy;

    [ObservableProperty]
    double _latitude;

    [ObservableProperty]
    double _longitude;

    [ObservableProperty]
    string _area;

    [ObservableProperty]
    double _size;

    [ObservableProperty]
    string _title;

    [ObservableProperty]
    EnumItem _selectedSoilType;

    [ObservableProperty] MapSpan _mapSpan;

    [ObservableProperty] Sensors.Location _labelPoint;

    [ObservableProperty] private bool _isCheckingLocation;

    [RelayCommand]
    async Task SubmitAsync()
    {
        var soilType = Boundaries;
        await Task.Delay(TimeSpan.FromSeconds(1));
        Debug.WriteLine("DEBUG INFO: Submitted");
    }

    [RelayCommand]
    void AddPoint()
    {
        Boundaries.Add(CurrentLocation);
        CurrentLocation = null;

        if (Boundaries.Count > 2)
        {
            var polygonBuilder = new PolygonBuilder(SpatialReferences.Wgs84);

            var coords = new List<Coordinate>()
            {
                new () { Latitude = -18.770168306913888, Longitude = 32.23066517235199 },
                    new () { Latitude =  -18.770444432902877, Longitude = 32.231199253347256 },
                    new () { Latitude = -18.77082368954858, Longitude = 32.23177549863135 },
                    new () { Latitude = -18.769772414647107, Longitude = 32.23212335401618 },
                    new () { Latitude = -18.769057144748274, Longitude = 32.23228849748119 },
                    new () { Latitude = -18.768844226610582, Longitude = 32.23137493788491 },
                    new () { Latitude = -18.770168306913888, Longitude = 32.23066517235199 },
            };

            foreach (var boundary in coords)
            {
                polygonBuilder.AddPoint(boundary.Latitude, boundary.Longitude);
            }

            var poly = polygonBuilder.ToGeometry();

            var area = GeometryEngine.AreaGeodetic(poly, AreaUnits.Hectares, GeodeticCurveType.Geodesic);
            var labelPoint = GeometryEngine.LabelPoint(poly);
            LabelPoint = new Sensors.Location(labelPoint.X, labelPoint.Y);
            Area = $"{area:N2}Ha";
            Size = area;

        }
        else if (Boundaries.Count == 2)
        {
            var loc = Boundaries[0];
            var distance = loc.CalculateDistance(Boundaries[1], DistanceUnits.Kilometers);
            Area = $"{(distance * 1000):N2}m";
        }
    }

    [RelayCommand]
    private async Task InitAsync()
    {
        try
        {
            IsCheckingLocation = true;

            var location = await _locationService.GetCurrentLocation();

            if (location != null)
            {
                Accuracy = location.Accuracy ?? 0;
                Latitude = location.Latitude;
                Longitude = location.Longitude;
                CurrentLocation = location;

                MapSpan = new MapSpan(location, 0.01, 0.01);
            }
        }
        catch (Exception ex)
        {
            // Unable to get location
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            IsCheckingLocation = false;
        }
    }

    [RelayCommand]
    async Task CancelAsync()
    {
        Area = "0Ha";
        Boundaries.Clear();
        Size = 0;
        Title = string.Empty;

        await CreateFieldSheet.DismissAsync();
        await Shell.Current.Navigation.PopAsync();
    }


    private async Task OnTimedEvent(Object source, EventArgs e)
    {
        var location = await _locationService.GetCurrentLocation();

        if ((location != null && Accuracy > location.Accuracy) || CurrentLocation == null)
        {
            Accuracy = location.Accuracy ?? 0;
            Latitude = location.Latitude;
            Longitude = location.Longitude;
            CurrentLocation = location;

            var boundaries = Boundaries.ToList();

            var coords = new List<Coordinate>(){
                new () { Latitude = 32.23066517235199,  Longitude = -18.770168306913888 },
                new () { Latitude = 32.231199253347256, Longitude =  -18.770444432902877 },
                new () { Latitude = 32.23177549863135, Longitude = -18.77082368954858 },
                new () { Latitude = 32.23212335401618, Longitude = -18.769772414647107 },
                new () { Latitude = 32.23228849748119, Longitude = -18.769057144748274 },
                new () { Latitude = 32.23137493788491, Longitude = -18.768844226610582 },
                new () { Latitude = 32.23066517235199, Longitude = -18.770168306913888 },
            };

            //MapSpan = new MapSpan(location, 0.01, 0.01);
        }
    }
}