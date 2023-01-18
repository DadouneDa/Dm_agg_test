namespace DmAggregator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BackgroundMessageSenderConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public const string SectionName = "bgSenderConfig";

        /// <summary>
        /// Maximum time in seconds to hold entries before sending aggregation.
        /// Note that this value is common to all queues, because full entries must always be sent before short entries.
        /// </summary>
        public int TimeThresholdSeconds { get; set; } = 30;
    }
}
