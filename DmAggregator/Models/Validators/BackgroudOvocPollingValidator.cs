using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class BackgroudOvocPollingValidator : AbstractValidator<BackgroudOvocPollingConfig>
    {
        /// <summary>
        /// 
        /// </summary>
        public BackgroudOvocPollingValidator()
        {
            RuleFor(x => x.PollingPeriodSeconds).InclusiveBetween(10, 3600);
        }
    }
}
