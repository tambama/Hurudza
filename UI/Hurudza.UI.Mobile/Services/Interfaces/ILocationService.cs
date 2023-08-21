namespace Hurudza.UI.Mobile.Services.Interfaces;

public interface ILocationService
{
    Task<Location> GetCurrentLocation();
}