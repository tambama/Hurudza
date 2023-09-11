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
}