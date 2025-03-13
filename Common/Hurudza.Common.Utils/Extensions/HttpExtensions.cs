using System.Text.Json;

namespace Hurudza.Common.Utils.Extensions;

public static class HttpContentExtensions
{
    public static async Task<T> ReadAsAsync<T>(this HttpContent content, System.Threading.CancellationToken cancellationToken)
    {
        return await JsonSerializer.DeserializeAsync<T>(await content.ReadAsStreamAsync(cancellationToken), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken: cancellationToken);
    }

    public static async Task<T> ReadAsAsync<T>(this HttpContent content)
    {
        return await JsonSerializer.DeserializeAsync<T>(await content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
