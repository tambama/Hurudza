using Hurudza.UI.Mobile.Models;

namespace Hurudza.UI.Mobile.Services.Interfaces
{
    public interface IProvinceService
    {
        event EventHandler<ServiceEventArgs<Province>> ProvincesUpdated;

        Task<IEnumerable<Province>> GetItemsAsync();
        Task RefreshItemsAsync();
        Task RemoveItemAsync(Province item);
        Task SaveItemAsync(Province item);
    }
}
