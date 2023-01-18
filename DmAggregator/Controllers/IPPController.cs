using DmAggregator.Aggregation;
using DmAggregator.Hashing;
using DmAggregator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace DmAggregator.Controllers
{
    /// <summary>
    /// IP Phone Controller
    /// Note - Two different routes are required because optional profile is not at the end
    /// </summary>
    [ApiController]
    [Route(HttpRouteConstants.IPPPhoneMgrStatusBase)]
    [Route(HttpRouteConstants.IPPPhoneMgrStatusBaseWithoutProfile)]
    public class IPPController : ControllerBase
    {
        private readonly Aggregator _aggregator;

        /// <summary>
        /// 
        /// </summary>
        public IPPController(Aggregator aggregator)
        {
            this._aggregator = aggregator;
        }

        /// <summary>
        /// IPP Init/Keep-Alive
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="profileId">Optional </param>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost(HttpRouteConstants.KeepAlivePath)]
        [HttpPost(HttpRouteConstants.InitPath)]
        [AllowAnonymous]
        public async Task<Results<Ok<IppKeepAliveResponse>, UnprocessableEntity<string>, NotFound<string>>> 
            IPPKeepAlive(string customerId, IPPKeepAliveRequest req, string? profileId = null)
        {
            // MAC should always be lower case, because OVOC converts it.
            // Otherwise, we will not find the MAC for an action from OVOC.
            // MAC is validated for non-null
            req.MAC = req.MAC.ToLower();

            // set request params
            // Note that even if these properties are set by a hacked request, they are overwritten below
            req.CustomerId = customerId;
            req.ProfileId = profileId;
            req.ExternalIp = this.HttpContext.Connection.RemoteIpAddress?.ToString();
            req.OriginalUrl = this.Request.Path.ToString();

            var response = await this._aggregator.AddEntry(req);

            return response;
        }

    }
}
