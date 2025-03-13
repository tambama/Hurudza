namespace Hurudza.UI.Web.Services.Interfaces;

public interface IMapBoxJsInterop
{
    Task LoadMap(double[] center);
}