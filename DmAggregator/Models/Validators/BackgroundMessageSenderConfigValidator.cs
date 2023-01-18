using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class BackgroundMessageSenderConfigValidator : AbstractValidator<BackgroundMessageSenderConfig>
    {
        /// <summary>
        /// 
        /// </summary>
        public BackgroundMessageSenderConfigValidator()
        {
            RuleFor(x => x.TimeThresholdSeconds).InclusiveBetween(10, 300);
        }
    }
}
