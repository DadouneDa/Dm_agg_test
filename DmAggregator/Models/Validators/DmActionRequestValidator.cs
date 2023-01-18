using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class DmActionRequestValidator : AbstractValidator<DmActionRequest>
    {
        /// <summary>
        /// 
        /// </summary>
        public DmActionRequestValidator()
        {
            RuleFor(x => x.Mac).NotEmpty();
            RuleFor(x => x.Body).NotEmpty();
            RuleFor(x => x.Expiredin).InclusiveBetween(10, 24 * 3600);
            RuleFor(x => x.Operator).NotEmpty();
            RuleFor(x => x.Action).NotEmpty();

            RuleFor(x => x.ExpiredUtcTicks).Empty().WithMessage($"{nameof(DmActionRequest.ExpiredUtcTicks)} must not be set in request");
        }
    }
}
