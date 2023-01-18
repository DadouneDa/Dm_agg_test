using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class BackgroundSenderQueueConfigValidator : AbstractValidator<BackgroundSenderQueueConfig>
    {
        /// <summary>
        /// 
        /// </summary>
        public BackgroundSenderQueueConfigValidator()
        {
            RuleFor(x => x.Capacity).InclusiveBetween(10, 2000);
            RuleFor(x => x.NumThreshold).InclusiveBetween(1, 100);
            RuleFor(x => x).Must(x => x.NumThreshold <= x.Capacity)
                .WithMessage(x => $"'{nameof(x.NumThreshold)}' cannot be larger than '{nameof(x.Capacity)}'");
        }
    }
}
