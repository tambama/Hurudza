namespace Hurudza.UI.Shared.Services.Interfaces;

public interface IMapBoxJsInterop
{
    Task LoadMap(double[] center);
}