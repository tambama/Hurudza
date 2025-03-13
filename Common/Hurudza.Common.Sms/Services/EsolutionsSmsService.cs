using System.Net.Http.Headers;
using System.Text;
using Hurudza.Common.Sms.Esolutions.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Hurudza.Common.Sms.Services;

public class EsolutionsSmsService: ISmsService
{
    public async Task<string> Send(string to, string message)
    {
        try
        {
            var smsRequest = new SmsRequest
            {
                Originator = "Zesaprepaid",
                Destination = to,
                MessageText = message,
                MessageReference = Guid.NewGuid().ToString(),
                MessageDate = DateTime.Now.ToString("YYYYMMddHHmmss"),
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", "UkVZQ0FSWUFQSTp0WHZiN0U3WQ==");
            client.Timeout = TimeSpan.FromMinutes(1);

            var json = JsonConvert.SerializeObject(
                smsRequest,
                new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                RequestUri = new Uri("https://mobile.esolutions.co.zw/bmg/api/single")
            };

            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var smsResponse = JsonConvert.DeserializeObject<SmsResponse>(responseString);

            return smsResponse?.Status ?? string.Empty;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return "Failed";
        }
    }
}