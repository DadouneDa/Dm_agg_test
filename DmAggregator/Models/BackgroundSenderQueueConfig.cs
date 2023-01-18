namespace DmAggregator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BackgroundSenderQueueConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public const string FullQConfigectionName = "fullQConfig";

        /// <summary>
        /// 
        /// </summary>
        public const string ShortQConfigSectionName = "shortQConfig";

        /// <summary>
        /// Queue capacity. If capacity is reached then oldest messages are discarded
        /// </summary>
        public int Capacity { get; set; } = 1000;

        /// <summary>
        /// Maximum number of entries to hold before sending aggregation
        /// </summary>
        public int NumThreshold { get; set; } = 50;
    }
}
