using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class AggregatorConfigValidator : AbstractValidator<AggregatorConfig>
    {
        /// <summary>
        /// 
        /// </summary>
        public AggregatorConfigValidator()
        {
            RuleFor(x => x.CacheTtlSeconds).InclusiveBetween(10, 24 * 3600);
        }
    }
}
