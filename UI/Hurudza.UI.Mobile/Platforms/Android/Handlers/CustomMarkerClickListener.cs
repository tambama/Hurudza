using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace Hurudza.UI.Mobile;

internal class CustomMarkerClickListener : Java.Lang.Object, GoogleMap.IOnMarkerClickListener
{
    private readonly CustomMapHandler mapHandler;

    public CustomMarkerClickListener(CustomMapHandler mapHandler)
    {
        this.mapHandler = mapHandler;
    }

    public bool OnMarkerClick(Marker marker)
    {
        var pin = mapHandler.Markers.FirstOrDefault(x => x.marker.Id == marker.Id);
        pin.pin?.SendMarkerClick();
        marker.ShowInfoWindow();
        return true;
    }
}