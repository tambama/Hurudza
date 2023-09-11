using Hurudza.UI.Mobile.ViewModels.Farms;

namespace Hurudza.UI.Mobile.Pages.Farms;

public partial class FarmsPage : ContentPage
{
    private readonly FarmsViewModel _viewModel;

    public FarmsPage(FarmsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}