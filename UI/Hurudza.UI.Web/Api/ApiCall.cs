using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Hurudza.Common.Utils.Extensions;
using Hurudza.UI.Shared.Api.Interfaces;
using Serilog;

namespace Hurudza.UI.Web.Api;

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
        try
        {
            var token = await _localStorageService.GetItemAsync<string>("token");
            var client = _httpClientFactory.CreateClient("Api.Core");
        
            if (!string.IsNullOrEmpty(token))
            {
                // Explicit header setting instead of relying on extension method
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
                // Log token validity for debugging (remove in production)
                bool isValidJwt = IsValidJwtToken(token);
                Log.Information($"Using token (valid: {isValidJwt})");
            
                if (!isValidJwt)
                {
                    Log.Warning("Token appears to be invalid, consider refreshing authentication");
                }
            }
            else
            {
                Log.Information("No token found in local storage");
            }

            return client;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating authenticated HTTP client");
            // Return a basic client if authentication fails
            return _httpClientFactory.CreateClient("Api.Core");
        }
    }

    // Helper method to validate JWT token format (simple check)
    private bool IsValidJwtToken(string token)
    {
        // Simple structure validation (doesn't check signature)
        if (string.IsNullOrEmpty(token)) return false;
    
        var parts = token.Split('.');
        return parts.Length == 3; // Header, payload, signature
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
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
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
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
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
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
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
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
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
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
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
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
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
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
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
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
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
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
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
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
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
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
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
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return default;
        }
    }

    #endregion
}