using System.Net.Http.Json;
using Hurudza.Common.Utils.Extensions;
using Hurudza.UI.Web.Api.Interfaces;
using Hurudza.UI.Web.Models;

namespace Hurudza.UI.Web.Api;

public class ApiCall : IApiCall
{
    private const string ApiVersion = "1.0";
    private readonly IHttpClientFactory _httpClientFactory;
    
    public ApiCall(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<HttpClient> GetHttpClient()
    {
        var client = _httpClientFactory.CreateClient("Api.Core");

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

        public async Task<ApiResponse<T>> Add<T>(HttpClient client, string url, T entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PostAsJsonAsync($"{url}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApiResponse<T>> Add<T, TU>(HttpClient client, string url, TU entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PostAsJsonAsync($"{url}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApiResponse<T>> Update<T>(HttpClient client, string url, T entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PutAsJsonAsync($"{url}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApiResponse<T>> Update<T>(HttpClient client, string url, string id, T entity)
        {
            try
            {
                using (client)
                {
                    var response = await client.PutAsJsonAsync($"{url}/{id}?api-version={ApiVersion}", entity);

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, int id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, long id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, Guid id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, string id)
        {
            try
            {
                using (client)
                {
                    var response = await client.DeleteAsync($"{url}{(url.EndsWith("/") ? string.Empty : "/")}{id}?api-version={ApiVersion}");

                    var result = await response.Content.ReadAsAsync<ApiResponse<T>>();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
}