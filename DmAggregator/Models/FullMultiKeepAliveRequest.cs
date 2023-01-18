namespace DmAggregator.Models
{
    /// <summary>
    /// Multi full keep alive message sent to OVOC
    /// </summary>
    public class FullMultiKeepAliveRequest
    {
        /// <summary>
        /// Full keep alive requests
        /// </summary>
        public required IEnumerable<IPPKeepAliveRequest> Requests { get; set; }
    }
}
