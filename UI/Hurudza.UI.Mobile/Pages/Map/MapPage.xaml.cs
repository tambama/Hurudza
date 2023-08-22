using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hurudza.UI.Mobile.Services.Interfaces;
using Hurudza.UI.Mobile.ViewModels.Location;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using ServiceProvider = Hurudza.UI.Mobile.Extensions.ServiceProvider;

namespace Hurudza.UI.Mobile.Pages.Map;

public partial class MapPage : ContentPage
{
    private readonly ILocationService _locationService;

    public MapPage(MapPageViewModel vm, ILocationService locationService)
    {
        InitializeComponent();

        BindingContext = vm;
        _locationService = locationService;
    }

    private async void Map_OnLoaded(object sender, EventArgs e)
    {
        var location = await _locationService.GetCurrentLocation();
        
        var mapSpan = new MapSpan(location, 0.01, 0.01);
        
        map.MoveToRegion(mapSpan);
    }
}