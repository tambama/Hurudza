using Hurudza.UI.Mobile.Helpers;
using Hurudza.UI.Mobile.Models;
using Hurudza.UI.Mobile.ViewModels.Farms;
using ServiceProvider = Hurudza.UI.Mobile.Extensions.ServiceProvider;

namespace Hurudza.UI.Mobile.Pages.Farms;

public partial class CreateFarmPage : ContentPage
{
    private readonly FarmsViewModel _farmsViewModel;

    public CreateFarmPage()
	{
		InitializeComponent();
        _farmsViewModel = ServiceProvider.GetService<FarmsViewModel>();
        BindingContext = _farmsViewModel;
    }

    private void Provinces_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var province = provinces.SelectedItem as Province;
        _farmsViewModel.FilterDistricts(province.Id);
    }

    private void Districts_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var district = districts.SelectedItem as District;
        _farmsViewModel.FilterLocalAuthorities(district.Id);
    }

    private void LocalAuthorities_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var localAuthority = localAuthorities.SelectedItem as LocalAuthority;
        _farmsViewModel.FilterWards(localAuthority.Id);
    }
}