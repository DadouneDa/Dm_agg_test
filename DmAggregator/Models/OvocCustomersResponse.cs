namespace DmAggregator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OvocCustomersResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<OvocCustomerEntry> Customers { get; set; } = default!;
    }
}
