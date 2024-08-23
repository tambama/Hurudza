using Hurudza.UI.Mobile.Pages.Map;
using System.Diagnostics;
using Microsoft.Maui.Devices;

namespace Hurudza.UI.Mobile;

public partial class App : Application
{
    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjY2NjQ3OEAzMjMyMmUzMDJlMzBTdTFxOERnOHhHYkZUNktSRzFtRjhKYTBwWGdSOWNpdjdmK0pWc3J1ZUZJPQ==");

        InitializeComponent();
        
        //App.Current.UserAppTheme = AppTheme.Dark;

        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            Shell.Current.CurrentItem = PhoneTabs;

        //Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute("fields/createfieldonmap", typeof(CreateFieldPage));
    }

    async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
    {
        try { 
            await Shell.Current.GoToAsync($"///settings");
        }catch (Exception ex) {
            Debug.WriteLine($"err: {ex.Message}");
        }
    }
}