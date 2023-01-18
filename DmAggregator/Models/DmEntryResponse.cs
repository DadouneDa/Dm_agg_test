namespace DmAggregator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DmEntryResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public required string Mac { get; set; }

        /// <summary>
        /// Cached data for this mac
        /// </summary>
        public required RedisIppEntry Data { get; set; }
    }
}
