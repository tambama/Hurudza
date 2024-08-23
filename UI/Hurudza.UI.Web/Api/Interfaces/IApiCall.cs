namespace Hurudza.UI.Shared.Api.Interfaces;

public interface IApiCall
{
    Task<HttpClient> GetHttpClient();
    Task<T> Get<T>(HttpClient client, string url);
    Task<T> Get<T>(HttpClient client, string url, int id);
    Task<T> Get<T>(HttpClient client, string url, long id);
    Task<T> Get<T>(HttpClient client, string url, Guid id);
    Task<T> Get<T>(HttpClient client, string url, string id);
    Task<T> Add<T, TU>(HttpClient client, string url, TU entity);
    Task<T> Update<T, TU>(HttpClient client, string url, TU entity);
    Task<T> Update<T, TU>(HttpClient client, string url, string id, TU entity);
    Task<T> Remove<T>(HttpClient client, string url, int id);
    Task<T> Remove<T>(HttpClient client, string url, long id);
    Task<T> Remove<T>(HttpClient client, string url, Guid id);
    Task<T> Remove<T>(HttpClient client, string url, string id);
}