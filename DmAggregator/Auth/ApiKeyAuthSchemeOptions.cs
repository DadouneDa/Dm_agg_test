using Microsoft.AspNetCore.Authentication;

namespace DmAggregator.Auth
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public const string AuthSchemeName = "ApiKey";

        /// <summary>
        /// 
        /// </summary>
        public const string SectionName = "apiKeyAuth";

        /// <summary>
        /// 
        /// </summary>
        public const string ApiKeyHeaderName = "x-api-key";

        /// <summary>
        /// 
        /// </summary>
        public string HeaderName { get; set; } = ApiKeyHeaderName;

        /// <summary>
        /// 
        /// </summary>
        public string? ApiKey { get; set; }
    }
}
