using DmAggregator.Models;
using DmAggregator.Services.Ovoc;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace DmAggregator.Controllers
{
    /// <summary>
    /// Test OVOC controller
    /// </summary>
    [ApiController]
    [Route("api/ovocTest")]
    public class OvocTestController : ControllerBase
    {
        private readonly IOvocService _ovocService;
        private readonly OvocCache _ovocCache;
        private readonly OvocServiceConfig _ovocServiceConfig;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ovocService"></param>
        /// <param name="ovocCache"></param>
        /// <param name="ovocServiceConfig"></param>
        public OvocTestController(IOvocService ovocService, OvocCache ovocCache, OvocServiceConfig ovocServiceConfig)
        {
            this._ovocService = ovocService;
            this._ovocCache = ovocCache;
            this._ovocServiceConfig = ovocServiceConfig;
        }

        /// <summary>
        /// Return the CACHED Ovoc polling as last read from background service
        /// </summary>
        /// <returns></returns>
        [HttpGet("ovocCache")]
        public OvocCache GetOvocCache()
        {
            return this._ovocCache;
        }

        /// <summary>
        /// Get DM customers
        /// </summary>
        /// <returns></returns>
        [HttpGet("dmCustomers")]
        public async Task<OvocCustomersResponse> GetOvocDmCustomers()
        {
            return await this._ovocService.GetDmCustomers();
        }

        /// <summary>
        /// Get OVOC service configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet("config")]
        public OvocServiceConfig GetOvocServiceConfig()
        {
            return this._ovocServiceConfig;
        }

        /// <summary>
        /// Add customer ID for testing. Will be deleted next time Ovoc is polled
        /// </summary>
        /// <param name="customerId"></param>
        [HttpPost("dmCustomers/{customerId}")]
        public void TmpAddDmCustomer(string customerId)
        {
            var customers = this._ovocCache.Customers;
            if (customers == null)
            {
                this._ovocCache.Customers = new OvocCustomerEntry[] { new OvocCustomerEntry { CustomerId = customerId } };
            }
            else
            {
                var tmp = new List<OvocCustomerEntry>(customers);
                tmp.Add(new OvocCustomerEntry { CustomerId = customerId });
                this._ovocCache.Customers = tmp.ToArray();
            }
        }

        /// <summary>
        /// Get rejected endpoints from Ovoc
        /// </summary>
        /// <returns></returns>
        [HttpGet("rejectedEndpoints")]
        public async Task<OvocRejectedEndpointsResponse> GetOvocRejectedEndpoints()
        {
            return await this._ovocService.GetOvocRejectedEndpoints();
        }


        /// <summary>
        /// Add rejected endpoint for testing. Will be deleted next time Ovoc is polled
        /// </summary>
        /// <returns></returns>
        [HttpPost("rejectedEndpoints/{mac}")]
        public void TmpAddRejectedMacs(string mac)
        {
            var rejected = this._ovocCache.RejectedMacs;
            if (rejected == null)
            {
                this._ovocCache.RejectedMacs = new ConcurrentDictionary<string, object?> { [mac] = null};
            }
            else
            {
                rejected.TryAdd(mac, null);
            }
        }
    }
}
