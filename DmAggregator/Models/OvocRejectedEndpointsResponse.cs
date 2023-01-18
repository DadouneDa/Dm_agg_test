namespace DmAggregator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OvocRejectedEndpointsResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<OvocRejectedEndpointEntry> RejectedEndpoints { get; set; } = default!;
    }
}
