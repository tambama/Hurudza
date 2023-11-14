using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hurudza.UI.Mobile.Services.Interfaces;
using Hurudza.UI.Mobile.Models;
using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IProvinceService _provinceService;
        [ObservableProperty] bool _isRefreshing;
        [ObservableProperty] ConcurrentObservableCollection<Province> _provinces;

        public HomeViewModel(IProvinceService provinceService)
        {
            _provinceService = provinceService;
            Provinces = new ConcurrentObservableCollection<Province>();
            _provinceService.ItemsUpdated += OnProvincesUpdated;
        }

        [RelayCommand]
        public async Task RefreshProvincesAsync()
        {
            if (IsRefreshing) return;
            IsRefreshing = true;

            try
            {
                await _provinceService.RefreshItemsAsync();
                var provinces = await _provinceService.GetItemsAsync();
                Provinces.ReplaceAll(provinces);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Refresh Provinces", ex.Message, "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async void OnProvincesUpdated(object sender, ServiceEventArgs<Province> e)
        {
            switch (e.Action)
            {
                case ServiceEventArgs<Province>.ListAction.Add:
                    Provinces.AddIfMissing(m => m.Id == e.Item.Id, e.Item);
                    break;
                case ServiceEventArgs<Province>.ListAction.Delete:
                    Provinces.RemoveIf(m => m.Id == e.Item.Id);
                    break;
                case ServiceEventArgs<Province>.ListAction.Update:
                    Provinces.ReplaceIf(m => m.Id == e.Item.Id, e.Item);
                    break;
            }
        }
    }
}
