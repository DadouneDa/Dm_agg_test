namespace DmAggregator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BackgroudOvocPollingConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public const string SectionName = "ovocPolling";

        /// <summary>
        /// 
        /// </summary>
        public int PollingPeriodSeconds { get; set; } = 300;
    }
}
