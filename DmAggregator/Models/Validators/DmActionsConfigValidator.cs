using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class DmActionsConfigValidator : AbstractValidator<DmActionsConfig>
    {
        /// <summary>
        /// 
        /// </summary>
        public DmActionsConfigValidator()
        {
            RuleFor(x => x.MaxIppActions).InclusiveBetween(1, 10);
        }
    }
}
