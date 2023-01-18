using Microsoft.AspNetCore.Authentication;

namespace DmAggregator.Auth
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApiKeyAuthExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApiKeyAuthentication(this AuthenticationBuilder builder, Action<ApiKeyAuthSchemeOptions> configureOptions)
        {
            return builder.AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeyAuthSchemeOptions.AuthSchemeName, "API Key auth", configureOptions);
        }
    }
}
