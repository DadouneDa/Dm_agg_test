namespace DmAggregator.Models
{

    /// <summary>
    /// Multi short keep alive message sent to OVOC
    /// </summary>
    public class ShortMultiKeepAliveRequest
    {
        /// <summary>
        /// MAC addresses
        /// </summary>
        public required IEnumerable<string> Endpoints { get; set; }
    }
}
