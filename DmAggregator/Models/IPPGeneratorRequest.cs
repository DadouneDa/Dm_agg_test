namespace DmAggregator.Models
{
    /// <summary>
    /// IPP keep alive generator request
    /// </summary>
    public class IPPGeneratorRequest
    {
        /// <summary>
        /// Randomizer seed.
        /// If not null then same random data will be sent with each request with the same seed value.
        /// If null then different random data will be sent with each request.
        /// </summary>
        /// <example>875644</example>
        public int? Seed { get; set; } = 875644;

        /// <summary>
        /// Number of IPP to generate
        /// </summary>
        /// <example>3</example>
        public int NumIPP { get; set; } = 3;

        /// <summary>
        /// Number of times to repeat IPP messages
        /// </summary>
        /// <example>1</example>
        public int NumLoops { get; set; } = 1;

        /// <summary>
        /// User ID start
        /// </summary>
        /// <example>1000</example>
        public int UserIdStart { get; set; } = 1000;

        /// <summary>
        /// Customer ID
        /// </summary>
        /// <example>a4bc9bc5-3240-483e-bd77-b19b25e9e00a</example>
        public string CustomerId { get; set; } = "a4bc9bc5-3240-483e-bd77-b19b25e9e00a";
    }
}
