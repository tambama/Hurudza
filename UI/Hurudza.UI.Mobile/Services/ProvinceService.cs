using Hurudza.UI.Mobile.Models;
using Hurudza.UI.Mobile.Services.Interfaces;
using Microsoft.Datasync.Client;
using Microsoft.Datasync.Client.SQLiteStore;

namespace Hurudza.UI.Mobile.Services
{
    public class ProvinceService : IProvinceService
    {
        private DatasyncClient _dataSyncClient = null;
        private IOfflineTable<Province> _provincesTable = null;
        public string OfflineDb { get; set; }
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _asyncLock = new(1, 1);
        public event EventHandler<ServiceEventArgs<Province>> ProvincesUpdated;
        public Func<Task<AuthenticationToken>> TokenRequestor;
        public Uri ServiceUri { get; set; } 

        public ProvinceService()
        {
            ServiceUri = new Uri("https://xj4wzbkj-7148.uks1.devtunnels.ms/");
        }

        private async Task InitializeAsync()
        {
            // Short circuit, in case we are already initialized.
            if (_isInitialized)
            {
                return;
            }

            try
            {
                // Wait to get the async initialization lock
                await _asyncLock.WaitAsync();
                if (_isInitialized)
                {
                    // This will also execute the async lock.
                    return;
                }

                var connectionString = new UriBuilder
                {
                    Scheme = "file",
                    Path = OfflineDb,
                    Query = "?mode=rwc"
                }.Uri.ToString();
                var store = new OfflineSQLiteStore(connectionString);
                store.DefineTable<Province>();
                var options = new DatasyncClientOptions
                {
                    HttpPipeline = new HttpMessageHandler[] { new LoggingHandler() },
                    OfflineStore = store
                };

                // Initialize the client.
                _dataSyncClient = TokenRequestor == null
                    ? new DatasyncClient(ServiceUri, options)
                    : new DatasyncClient(ServiceUri, new GenericAuthenticationProvider(TokenRequestor), options);
                await _dataSyncClient.InitializeOfflineStoreAsync();

                // Get a reference to the offline provincesTable.
                _provincesTable = _dataSyncClient.GetOfflineTable<Province>();

                // Set _initialied to true to prevent duplication of locking.
                _isInitialized = true;
            }
            catch (Exception)
            {
                // Re-throw the exception.
                throw;
            }
            finally
            {
                _asyncLock.Release();
            }
        }


        /// <summary>
        /// Get all the items in the list.
        /// </summary>
        /// <returns>The list of items (asynchronously)</returns>
        public async Task<IEnumerable<Province>> GetItemsAsync()
        {
            await InitializeAsync();
            return await _provincesTable.ToListAsync();
        }

        /// <summary>
        /// Refreshes the Provinces list manually.
        /// </summary>
        /// <returns>A task that completes when the refresh is done.</returns>
        public async Task RefreshItemsAsync()
        {
            await InitializeAsync();
            await _provincesTable.PushItemsAsync();
            await _provincesTable.PullItemsAsync();
            return;
        }

        /// <summary>
        /// Removes an item in the list, if it exists.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns>A task that completes when the item is removed.</returns>
        public async Task RemoveItemAsync(Province item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Id == null)
            {
                // Short circuit for when the item has not been saved yet.
                return;
            }
            await InitializeAsync();
            await _provincesTable.DeleteItemAsync(item);
            ProvincesUpdated?.Invoke(this, new ServiceEventArgs<Province>(ServiceEventArgs<Province>.ListAction.Delete, item));
        }

        /// <summary>
        /// Saves an item to the list.  If the item does not have an Id, then the item
        /// is considered new and will be added to the end of the list.  Otherwise, the
        /// item is considered existing and is replaced.
        /// </summary>
        /// <param name="item">The new item</param>
        /// <returns>A task that completes when the item is saved.</returns>
        public async Task SaveItemAsync(Province item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await InitializeAsync();

            ServiceEventArgs<Province>.ListAction action = (item.Id == null) ? ServiceEventArgs<Province>.ListAction.Add : ServiceEventArgs<Province>.ListAction.Update;
            if (item.Id == null)
            {
                await _provincesTable.InsertItemAsync(item);
            }
            else
            {
                await _provincesTable.ReplaceItemAsync(item);
            }
            ProvincesUpdated?.Invoke(this, new ServiceEventArgs<Province>(action, item));
        }
    }
}
