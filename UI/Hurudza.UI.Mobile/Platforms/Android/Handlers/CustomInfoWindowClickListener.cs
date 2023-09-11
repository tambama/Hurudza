using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace Hurudza.UI.Mobile;

internal class CustomInfoWindowClickListener : Java.Lang.Object, GoogleMap.IOnInfoWindowClickListener
{
    private readonly CustomMapHandler mapHandler;

    public CustomInfoWindowClickListener(CustomMapHandler mapHandler)
    {
        this.mapHandler = mapHandler;
    }

    public void OnInfoWindowClick(Marker marker)
    {
        var pin = mapHandler.Markers.FirstOrDefault(x => x.marker.Id == marker.Id);
        pin.pin?.SendInfoWindowClick();
    }
}