using Hurudza.UI.Web.Models;

namespace Hurudza.UI.Web.Api.Interfaces;

public interface IApiCall
{
    Task<HttpClient> GetHttpClient();
    Task<T> Get<T>(HttpClient client, string url);
    Task<T> Get<T>(HttpClient client, string url, int id);
    Task<T> Get<T>(HttpClient client, string url, long id);
    Task<T> Get<T>(HttpClient client, string url, Guid id);
    Task<T> Get<T>(HttpClient client, string url, string id);
    Task<ApiResponse<T>> Add<T>(HttpClient client, string url, T entity);
    Task<ApiResponse<T>> Add<T, TU>(HttpClient client, string url, TU entity);
    Task<ApiResponse<T>> Update<T>(HttpClient client, string url, T entity);
    Task<ApiResponse<T>> Update<T>(HttpClient client, string url, string id, T entity);
    Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, int id);
    Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, long id);
    Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, Guid id);
    Task<ApiResponse<T>> Remove<T>(HttpClient client, string url, string id);
}