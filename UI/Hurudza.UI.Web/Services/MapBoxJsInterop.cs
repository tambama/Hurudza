using Hurudza.UI.Web.Services.Interfaces;
using Microsoft.JSInterop;

namespace Hurudza.UI.Web.Services;

public class MapBoxJsInterop : IMapBoxJsInterop, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public MapBoxJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Hurudza.UI.Shared/js/services/mapboxJsInterop.js").AsTask());
    }

    public async Task LoadMap(double[] center)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("loadMap", center);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}