using System.Net.Http.Json;
using Blazored.LocalStorage;
using Hurudza.Common.Utils.Extensions;
using Hurudza.UI.Shared.Api.Interfaces;
using IdentityModel.Client;

namespace Hurudza.UI.Shared.Api;

public class ApiCall : IApiCall
{
    private const string ApiVersion = "1.0";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocalStorageService _localStorageService;

    public ApiCall(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _httpClientFactory = httpClientFactory;
        _localStorageService = localStorageService;
    }
    
    public async Task<HttpClient> GetHttpClient()
    {
        var token = await _localStorageService.GetItemAsync<string>("token");
        var client = _httpClientFactory.CreateClient("Api.Core");
        if (!string.IsNullOrEmpty(token))
        {
            client.SetBearerToken(token);
        }

        return await Task.FromResult(client);
    }
    
    #region ApiCalls

        public async Task<T> Get<T>(HttpClient client, string url)
        {
            try
            {
                using (client)
                {
                    var response = await client.GetAsync($"{url}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> Get<T>(HttpClient client, string url, int id)
        {
            try
            {
                using (client)
                {
                    var apiUrl = $"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}";
                    var response = await client.GetAsync(apiUrl);

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> Get<T>(HttpClient client, string url, long id)
        {
            try
            {
                using (client)
                {
                    var response = await client.GetAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");
                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> Get<T>(HttpClient client, string url, Guid id)
        {
            try
            {
                using (client)
                {
                    var response = await client.GetAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> Get<T>(HttpClient client, string url, string id)
        {
            try
            {
                using (client)
                {
                    var response = await client.GetAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<T> Add<T, TU>(HttpClient client, string url, TU entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PostAsJsonAsync($"{url}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (System.Exception ex)
            {
                return default;
            }
        }

        public async Task<T> Update<T, TU>(HttpClient client, string url, TU entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PutAsJsonAsync($"{url}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (System.Exception)
            {
                return default;
            }
        }

        public async Task<T> Update<T, TU>(HttpClient client, string url, string id, TU entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PutAsJsonAsync($"{url}/{id}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (System.Exception)
            {
                return default;
            }
        }

        public async Task<T> Remove<T>(HttpClient client, string url, int id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (System.Exception)
            {
                return default;
            }
        }

        public async Task<T> Remove<T>(HttpClient client, string url, long id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (System.Exception)
            {
                return default;
            }
        }

        public async Task<T> Remove<T>(HttpClient client, string url, Guid id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (System.Exception)
            {
                return default;
            }
        }

        public async Task<T> Remove<T>(HttpClient client, string url, string id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<T>();

                    return result;
                }
            }
            catch (System.Exception)
            {
                return default;
            }
        }

        #endregion
}