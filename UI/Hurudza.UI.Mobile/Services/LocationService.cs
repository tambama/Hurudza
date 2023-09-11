using System.Diagnostics;

namespace Hurudza.UI.Mobile.Services;

public partial class LocationService
{
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;

    public event EventHandler<Location> LocationChanged;
    public event EventHandler<string> StatusChanged;

    public async Task<Location> GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
                Debug.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

            return location;
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            // Unable to get location
            return default;
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    public void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }

    public void Initialize()
    {
#if ANDROID
        AndroidInitialize();
#elif IOS
        IosInitialize();
#endif
    }

    public void Stop()
    {
#if ANDROID
        AndroidStop();
#elif IOS
        IosStop();
#endif
    }

    protected virtual void OnLocationChanged(Location e)
    {
        LocationChanged?.Invoke(this, e);
    }

    protected virtual void OnStatusChanged(string e)
    {
        StatusChanged?.Invoke(this, e);
    }
}