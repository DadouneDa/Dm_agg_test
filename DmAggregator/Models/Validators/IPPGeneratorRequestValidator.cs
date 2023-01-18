using FluentValidation;

namespace DmAggregator.Models.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class IPPGeneratorRequestValidator : AbstractValidator<IPPGeneratorRequest>
    {
        /// <summary>
        /// 
        /// </summary>
        public IPPGeneratorRequestValidator()
        {
            RuleFor(x => x.NumIPP).InclusiveBetween(1, 100);
            RuleFor(x => x.NumLoops).InclusiveBetween(1, 100);
        }
    }
}
