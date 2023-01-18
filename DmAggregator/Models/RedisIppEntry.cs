namespace DmAggregator.Models
{
    /// <summary>
    /// This is entry, stored as JSON string, in Redis
    /// This was chosen over storing a native Redis Hash, because it's much more flexible,
    /// and there's no need to atomically update a single field.
    /// </summary>
    public class RedisIppEntry
    {
        /// <summary>
        /// 
        /// </summary>
        public byte[]? LastKeepAliveHash { get; set; }

        /// <summary>
        /// Actions from device manager that should be returned to IPP
        /// </summary>
        public List<DmActionRequest>? Actions { get; set; }
    }
}
