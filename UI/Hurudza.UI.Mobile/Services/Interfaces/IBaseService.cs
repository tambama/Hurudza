using Hurudza.UI.Mobile.Models;
using Microsoft.Datasync.Client;

namespace Hurudza.UI.Mobile.Services.Interfaces
{
    public interface IBaseService<T> where T : DatasyncClientData
    {
        event EventHandler<ServiceEventArgs<T>> ItemsUpdated;

        Task<IEnumerable<T>> GetItemsAsync();
        Task RefreshItemsAsync();
        Task RemoveItemAsync(T item);
        Task SaveItemAsync(T item);
    }
}
