using DmAggregator.Models;

namespace DmAggregator.Services.Ovoc
{
    /// <summary>
    /// Send messages to OVOC
    /// </summary>
    public interface IOvocService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task SendFullMultiKeepAlive(FullMultiKeepAliveRequest req);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task SendShortMultiKeepAlive(ShortMultiKeepAliveRequest req);

        /// <summary>
        /// Get DM customers
        /// </summary>
        /// <returns></returns>
        Task<OvocCustomersResponse> GetDmCustomers();

        /// <summary>
        /// Get rejected endpoints
        /// </summary>
        /// <returns></returns>
        Task<OvocRejectedEndpointsResponse> GetOvocRejectedEndpoints();
    }
}