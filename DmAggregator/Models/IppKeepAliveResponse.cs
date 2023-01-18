namespace DmAggregator.Models
{
    /// <summary>
    /// Response returned IPP Keep-Alive
    /// </summary>
    public class IppKeepAliveResponse
    {
        /// <summary>
        /// Array of actions as previously set in <see cref="DmActionRequest.Body"/>
        /// </summary>
        public required IEnumerable<string> Requests { get; set; }
    }
}
