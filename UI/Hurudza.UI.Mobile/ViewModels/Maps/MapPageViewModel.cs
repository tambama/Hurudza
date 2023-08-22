using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hurudza.UI.Mobile.Models;
using Hurudza.UI.Mobile.Services.Interfaces;
using Microsoft.Maui.Maps;

namespace Hurudza.UI.Mobile.ViewModels.Location;

public partial class MapPageViewModel : ObservableObject
{
    private readonly ILocationService _locationService;

    public MapPageViewModel(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [ObservableProperty] private int _pinCreatedCount = 0;
    
    ObservableCollection<Position> _positions;
    
    [ObservableProperty]
    Microsoft.Maui.Devices.Sensors.Location _currentLocation;

    [ObservableProperty] private MapSpan _mapSpan;

    [ObservableProperty] private bool _isCheckingLocation;

    [RelayCommand]
    async Task Submit()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        Debug.WriteLine("DEBUG INFO: Submitted");
    }

    [RelayCommand]
    private async Task InitAsync()
    {
        try
        {
            IsCheckingLocation = true;

            CurrentLocation = await _locationService.GetCurrentLocation();

            MapSpan = new MapSpan(CurrentLocation, 0.01, 0.01);
        }
        catch (Exception ex)
        {
            // Unable to get location
        }
        finally
        {
            IsCheckingLocation = false;
        }
    }
    
    [RelayCommand]
    async Task AddLocation()
    {
        _positions.Add(await NewPosition());
    }

    [RelayCommand]
    void RemoveLocation()
    {
        if (_positions.Any())
        {
            _positions.Remove(_positions.First());
        }
    }

    async Task<Position> NewPosition()
    {
        PinCreatedCount++;

        var currentLocation = await _locationService.GetCurrentLocation();
        
        return new Position(
            $"Pin {PinCreatedCount}",
            $"Desc {PinCreatedCount}",
            currentLocation);
    }
}