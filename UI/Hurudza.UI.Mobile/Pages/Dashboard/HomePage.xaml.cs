using Hurudza.UI.Mobile.ViewModels;

namespace Hurudza.UI.Mobile.Pages.Dashboard;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _homeViewModel;

    public HomePage(HomeViewModel homeViewModel)
    {
        InitializeComponent();
        _homeViewModel = homeViewModel;
        BindingContext = _homeViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //await _homeViewModel.RefreshProvincesCommand.ExecuteAsync(null);
    }
}