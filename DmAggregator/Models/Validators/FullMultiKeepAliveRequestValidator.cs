using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class FullMultiKeepAliveRequestValidator : AbstractValidator<FullMultiKeepAliveRequest>
    {
        /// <summary>
        /// 
        /// </summary>
        public FullMultiKeepAliveRequestValidator()
        {
            RuleFor(x => x.Requests).NotEmpty();
            RuleForEach(x => x.Requests)
                .ChildRules(x =>
                {
                    x.RuleFor(x => x.MAC).NotEmpty();
                })
                .When(x => x.Requests != null);
        }
    }
}
