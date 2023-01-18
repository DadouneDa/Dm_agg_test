using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class ShortMultiKeepAliveRequestValidator : AbstractValidator<ShortMultiKeepAliveRequest>
    {
        /// <summary>
        /// 
        /// </summary>
        public ShortMultiKeepAliveRequestValidator()
        {
            RuleFor(x => x.Endpoints).NotEmpty();
            RuleForEach(x => x.Endpoints)
                .ChildRules(x =>
                {
                    x.RuleFor(x => x).NotEmpty();
                })
                .When(x => x.Endpoints != null);
        }
    }
}
