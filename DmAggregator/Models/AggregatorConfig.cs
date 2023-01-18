namespace DmAggregator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AggregatorConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public const string SectionName = "Aggregator";

        /// <summary>
        /// Cache TTL in seconds before entry is deleted.
        /// Note - each cache get refreshes the TTL
        /// </summary>
        public int CacheTtlSeconds { get; set; } = 300;

        /// <summary>
        /// If true then all KeepAlive message properties are logged to Application Insights custom event properties.
        /// Otherise, only MAC address is logged.
        /// </summary>
        public bool FullKaCustomEvent { get; init; } = false;
    }
}
