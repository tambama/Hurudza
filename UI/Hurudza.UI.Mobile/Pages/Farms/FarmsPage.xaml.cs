using Hurudza.UI.Mobile.ViewModels.Farms;
using Hurudza.UI.Mobile.Models;

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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitCommand.ExecuteAsync(null);
    }

    private void Edit_Clicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var farm = menuItem?.CommandParameter as Farm;
        _viewModel.EditCommand.Execute(farm);
    }

    private void Delete_Clicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var farm = menuItem?.CommandParameter as Farm;
        _viewModel.DeleteCommand.Execute(farm);
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = e.NewTextValue;
        if (string.IsNullOrEmpty(text))
        {
            _viewModel.SearchFarmCommand.Execute(null);
        }
    }
}