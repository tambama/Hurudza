using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Hurudza.UI.Mobile.Data;
using Hurudza.UI.Mobile.Pages.Authentication;
using Hurudza.UI.Mobile.Pages.Map;
using Hurudza.UI.Mobile.Services;
using Hurudza.UI.Mobile.ViewModels.Location;
using Hurudza.UI.Shared.Services;
using Hurudza.UI.Shared.Services.Interfaces;
using Microsoft.Maui.LifecycleEvents;
using The49.Maui.BottomSheet;
using Syncfusion.Maui.Core.Hosting;
using Hurudza.UI.Mobile.Pages.Farms;
using Hurudza.UI.Mobile.ViewModels.Farms;
using Hurudza.UI.Shared.Api.Interfaces;
using Hurudza.UI.Shared.Api;
using Hurudza.UI.Web.Api.Interfaces;
using Hurudza.UI.Mobile.Services.Interfaces;
using Hurudza.UI.Mobile.Pages.Dashboard;
using Hurudza.UI.Mobile.ViewModels;
using Hurudza.UI.Mobile.Helpers;

namespace Hurudza.UI.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                fonts.AddFont("Roboto-Medium.ttf", "Roboto-Medium");
                fonts.AddFont("Roboto-Regular.ttf", "Roboto-Regular");
            })
            .UseMauiMaps()
            .UseMauiCommunityToolkit()
            .UseBottomSheet();

        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddHttpClient("Api.Core", client =>
        {
            client.BaseAddress = new Uri("https://xj4wzbkj-7148.uks1.devtunnels.ms/");
        });

        builder.ConfigureMauiHandlers((handlers) =>
        {
            //handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
        });
        
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
        builder.Services.AddSingleton<LocationService>();
        builder.Services.AddSingleton<IMapBoxJsInterop, MapBoxJsInterop>();
        builder.Services.AddSingleton<IApiCall, ApiCall>();
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<IProvinceService, ProvinceService>();
        builder.Services.AddSingleton<IDistrictService, DistrictService>();
        builder.Services.AddSingleton<ILocalAuthorityService, LocalAuthorityService>();
        builder.Services.AddSingleton<IWardService, WardService>();
        builder.Services.AddSingleton<IFarmService, FarmService>();
        builder.Services.AddSingleton<ICropService, CropService>();
        builder.Services.AddSingleton<IFieldCropService, FieldCropService>();
        builder.Services.AddSingleton<IFieldService, FieldService>();
        builder.Services.AddSingleton<ILocationsService, LocationsService>();
        
        // ViewModels
        builder.Services.AddSingleton<MapPageViewModel>();
        builder.Services.AddSingleton<CreateFieldOnMapViewModel>();
        builder.Services.AddSingleton<FarmsViewModel>();
        builder.Services.AddSingleton<HomeViewModel>();
        
        // Pages
        builder.Services.AddSingleton<MapPage>();
        builder.Services.AddSingleton<CreateFieldPage>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<FarmsPage>();
        builder.Services.AddSingleton<HomePage>();

        var app = builder.Build();

        return app;
    }
}