using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class IPPKeepAliveRequestValidator : AbstractValidator<IPPKeepAliveRequest>
    {
        /// <summary>
        /// 
        /// </summary>
        public IPPKeepAliveRequestValidator()
        {
            RuleFor(x => x.MAC).NotEmpty();
        }
    }
}
