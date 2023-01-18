using System.Collections.Concurrent;

namespace DmAggregator.Models
{
    /// <summary>
    /// The singleton containing last Ovoc customers and rejected MACS as read from the background service
    /// </summary>
    public class OvocCache
    {
        /// <summary>
        /// Array of non-null DM Ovoc customers
        /// </summary>
        public OvocCustomerEntry[]? Customers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? GetCustomersLastUpdateUtc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GetCustomersSuccessReads { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GetCustomersFailReads { get; set; }

        /// <summary>
        /// Original dictionary (null values) of rejected MACs
        /// </summary>
        public ConcurrentDictionary<string, object?>? RejectedMacsOrig { get; set; }

        /// <summary>
        /// Working copy of rejected Macs. They are deleted here once deleted from Redis
        /// </summary>
        public ConcurrentDictionary<string, object?>? RejectedMacs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? RejectedMacsLastUpdateUtc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RejectedMacsSuccessReads { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RejectedMacsFailReads { get; set; }

    }
}
