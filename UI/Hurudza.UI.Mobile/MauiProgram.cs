using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Hurudza.UI.Mobile.Data;
using Hurudza.UI.Mobile.Pages.Authentication;
using Hurudza.UI.Mobile.Pages.Map;
using Hurudza.UI.Mobile.Services;
using Hurudza.UI.Mobile.Services.Interfaces;
using Hurudza.UI.Mobile.ViewModels.Location;
using Hurudza.UI.Shared.Services;
using Hurudza.UI.Shared.Services.Interfaces;
using Microsoft.Maui.LifecycleEvents;

namespace Hurudza.UI.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
            })
            .UseMauiMaps()
            .UseMauiCommunityToolkit();

        builder.Services.AddMauiBlazorWebView();
        
        builder.ConfigureLifecycleEvents(lifecycle => {
#if WINDOWS
        //lifecycle
        //    .AddWindows(windows =>
        //        windows.OnNativeMessage((app, args) => {
        //            if (WindowExtensions.Hwnd == IntPtr.Zero)
        //            {
        //                WindowExtensions.Hwnd = args.Hwnd;
        //                WindowExtensions.SetIcon("Platforms/Windows/trayicon.ico");
        //            }
        //        }));

            lifecycle.AddWindows(windows => windows.OnWindowCreated((del) => {
                del.ExtendsContentIntoTitleBar = true;
            }));
#endif
        });

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddSingleton<MapBoxJsInterop>();
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddSingleton<IMapBoxJsInterop, MapBoxJsInterop>();
        
        // ViewModels
        builder.Services.AddSingleton<MapPageViewModel>();
        
        // Pages
        builder.Services.AddSingleton<MapPage>();
        builder.Services.AddSingleton<LoginPage>();

        var app = builder.Build();

        return app;
    }
}