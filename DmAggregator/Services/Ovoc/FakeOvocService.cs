using DmAggregator.Models;

namespace DmAggregator.Services.Ovoc
{
    /// <summary>
    /// 
    /// </summary>
    public class FakeOvocService : IOvocService
    {
        private readonly ILogger<FakeOvocService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public FakeOvocService(ILogger<FakeOvocService> logger)
        {
            this._logger = logger;
        }

        /// <inheritdoc/>
        public Task<OvocCustomersResponse> GetDmCustomers()
        {
            this._logger.LogWarning("Faking OVOC GetDmCustomers() !!");

            return Task.FromResult(new OvocCustomersResponse
            {
                Customers = new OvocCustomerEntry[] {
                    new OvocCustomerEntry 
                    {
                        CustomerId = "DummyCustomer"
                    }
                }
            });
        }

        /// <inheritdoc/>
        public Task<OvocRejectedEndpointsResponse> GetOvocRejectedEndpoints()
        {
            this._logger.LogWarning("Faking OVOC GetOvocRejectedEndpoints() !!");

            return Task.FromResult(new OvocRejectedEndpointsResponse
            {
                RejectedEndpoints = new OvocRejectedEndpointEntry[]
                {
                    new OvocRejectedEndpointEntry
                    {
                        Mac = "bad0bad1bad2"
                    } 
                }
            });
        }

        /// <inheritdoc/>
        public Task SendFullMultiKeepAlive(FullMultiKeepAliveRequest req)
        {
            this._logger.LogWarning($"Faking OVOC SendFullMultiKeepAlive() with {req.Requests.Count()} entries !!");
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SendShortMultiKeepAlive(ShortMultiKeepAliveRequest req)
        {
            this._logger.LogWarning($"Faking OVOC ShortMultiKeepAliveRequest() with {req.Endpoints.Count()} entries !!");
            return Task.CompletedTask;
        }
    }
}
