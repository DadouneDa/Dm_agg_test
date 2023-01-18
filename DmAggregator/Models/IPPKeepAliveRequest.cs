using System.Text.Json;
using System.Text.Json.Serialization;

namespace DmAggregator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class IPPKeepAliveRequest
    {
        /// <summary>
        /// MAC Address
        /// </summary>
        [JsonPropertyName("mac")]
        public required string MAC { get; set; }

        /// <summary>
        /// Set from request
        /// </summary>
        public string? ExternalIp { get; set; }

        /// <summary>
        /// Set from request URL
        /// </summary>
        public string? CustomerId { get; set; }

        /// <summary>
        /// Set from request URL - can be null
        /// </summary>
        public string? ProfileId { get; set; }

        /// <summary>
        /// SessionId and EmsUserPassword must be ignored when computing hash, because it is randomized in each request
        /// </summary>
        public string? SessionId { get; set; }

        /// <summary>
        /// SessionId and EmsUserPassword must be ignored when computing hash, because it is randomized in each request
        /// </summary>
        public string? EmsUserPassword { get; set; }

        /// <summary>
        /// Set from request URL
        /// </summary>
        public string? OriginalUrl { get; set; }

        /// <summary>
        /// Don't care about other properties, but they must be passed
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object>? ExtensionData { get; set; }
    }
}
