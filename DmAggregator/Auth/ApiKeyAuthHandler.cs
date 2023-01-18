using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DmAggregator.Auth
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthSchemeOptions>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        public ApiKeyAuthHandler(IOptionsMonitor<ApiKeyAuthSchemeOptions> options, ILoggerFactory logger,
                                 UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        /// <inheritdoc/>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Don't fail if AllowAnonymous is defined
            // It's only for skipping logging of authentication failure
            // Credit: https://stackoverflow.com/a/65235343/16404952
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (this.Request.Headers.TryGetValue(this.Options.HeaderName, out var headerValue)) 
            {
                if (headerValue == this.Options.ApiKey)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
                    var identity = new ClaimsIdentity(claims, this.Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }

            return Task.FromResult(AuthenticateResult.Fail("API KEY unauthorized"));
        }
    }
}
