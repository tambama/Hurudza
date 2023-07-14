using Newtonsoft.Json;

namespace Hurudza.Data.Models.Models
{
    public class ApiResponse
    {
        public int Status { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Title { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? StackTrace { get; }

        public Dictionary<int, List<string>>? ErrorCollection { get; set; }

        public ApiResponse(int statusCode, string? message = null, string? stackTrace = null, Dictionary<int, List<string>>? errorDescription = null)
        {
            Status = statusCode;
            Title = message ?? GetDefaultMessageForStatusCode(statusCode);
            StackTrace = stackTrace;
            ErrorCollection = errorDescription;
        }

        private static string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                404 => "Resource not found",
                500 => "An unhandled error occurred",
                _ => null
            };
        }
    }

    public class ApiResponse<T>
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("result")]
        public T? Result { get; set; }

        [JsonProperty("stackTrace")]
        public string? StackTrace { get; set; }
    }
}
