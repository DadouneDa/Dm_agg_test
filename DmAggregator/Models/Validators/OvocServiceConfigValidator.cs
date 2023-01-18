using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class OvocServiceConfigValidator : AbstractValidator<OvocServiceConfig>
    {
        /// <summary>
        /// 
        /// </summary>
        public OvocServiceConfigValidator()
        {
            RuleFor(x => x.BaseUrl).NotEmpty();

            RuleFor(x => x.BaseUrl)
                .Must(x =>
                {
                    Uri url = new Uri(x!);
                    
                    return
                        url.IsAbsoluteUri &&
                        (url.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) || url.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) &&
                        url.PathAndQuery == "/";

                })
                .When(x => x.BaseUrl != null);
        }
    }
}
