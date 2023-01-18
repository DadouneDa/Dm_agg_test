using System.Net.Http.Headers;
using System.Text.Json;

namespace DmAggregator.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions _defaultWebSerializerOptions =
            new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

        private static readonly MediaTypeHeaderValue _jsonContentTypeHeader = new MediaTypeHeaderValue("application/json")
        {
            CharSet = "utf-8"
        };

        /// <summary>
        /// POST as JSON not using chunked transfer encoding, for servers that do not support it
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostAsJsonNonChunkedAsync<TValue>(this HttpClient client, string? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            // Prepare a StringContent that already includes content-length, so chunked transfer encoding will not be used
            string json = JsonSerializer.Serialize(value, options ?? _defaultWebSerializerOptions);
            StringContent stringContent = new StringContent(json);
            stringContent.Headers.ContentType = _jsonContentTypeHeader;

            return client.PostAsync(requestUri, stringContent, cancellationToken);
        }
    }
}
