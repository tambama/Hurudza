using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hurudza.Common.Utils.Helpers;

public static class HttpHelpers
{
    public static HttpRequestMessage GetPostRequest<T>(T vendorBalanceRequest, string uri)
    {
        var json = JsonConvert.SerializeObject(
                        vendorBalanceRequest,
                        new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(uri),
        };

        request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
        return request;
    }
}
