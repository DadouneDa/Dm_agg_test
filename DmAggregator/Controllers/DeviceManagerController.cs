using DmAggregator.Aggregation;
using DmAggregator.Models;
using DmAggregator.Services.Redis;
using Microsoft.AspNetCore.Mvc;

namespace DmAggregator.Controllers
{
    /// <summary>
    /// Device manager controller
    /// </summary>
    [ApiController]
    [Route(HttpRouteConstants.DmBase)]
    public class DeviceManagerController : ControllerBase
    {
        private readonly ILogger<DeviceManagerController> _logger;
        private readonly DmActionsConfig _dmActionsConfig;
        private readonly AggregatorConfig _aggregatorConfig;
        private readonly IRedisCacheService _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dmActionsConfig"></param>
        /// <param name="aggregatorConfig"></param>
        /// <param name="cache"></param>
        public DeviceManagerController(
            ILogger<DeviceManagerController> logger,
            DmActionsConfig dmActionsConfig,
            AggregatorConfig aggregatorConfig,
            IRedisCacheService cache)
        {
            this._logger = logger;
            this._dmActionsConfig = dmActionsConfig;
            this._aggregatorConfig = aggregatorConfig;
            this._cache = cache;
        }

        /// <summary>
        /// Set IPP action
        /// </summary>
        /// <param name="dmActionRequest"></param>
        /// <returns></returns>
        [HttpPost(HttpRouteConstants.DmCmdPath)]
        public async Task SetIppAction(DmActionRequest dmActionRequest)
        {
            RedisIppEntry? redisIppEntry = await this._cache.GetObject<RedisIppEntry>(dmActionRequest.Mac!);

            if (redisIppEntry == null)
            {
                redisIppEntry = new RedisIppEntry();
            }

            if (redisIppEntry.Actions == null)
            {
                redisIppEntry.Actions = new List<DmActionRequest>();
            }

            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            dmActionRequest.ExpiredUtcTicks = utcNow
                .AddSeconds(dmActionRequest.Expiredin)
                .UtcTicks;

            // remove expired actions
            redisIppEntry.Actions.RemoveAll(x => x.ExpiredUtcTicks < utcNow.Ticks);

            // add this entry
            redisIppEntry.Actions.Add(dmActionRequest);

            // Remove excessive entry
            if (redisIppEntry.Actions.Count > this._dmActionsConfig.MaxIppActions)
            {
                redisIppEntry.Actions.RemoveAt(0);
            }

            await this._cache.SetObject(dmActionRequest.Mac!, redisIppEntry);

            // Set TTL - should be maximum of KA TTL and action expiry
            int ttlSeconds = Math.Max(dmActionRequest.Expiredin, this._aggregatorConfig.CacheTtlSeconds);
            await this._cache.KeyExpireAsync(dmActionRequest.Mac!, TimeSpan.FromSeconds(ttlSeconds));
        }

        /// <summary>
        /// Get multiple cache entries by comma separated macs
        /// </summary>
        /// <param name="macs">comma separated macs to get</param>
        /// <returns></returns>
        [HttpGet("macs/{macs}")]
        public async Task<IEnumerable<DmEntryResponse>> GetMacs(string macs)
        {
            var parts = macs.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var response = new List<DmEntryResponse>(parts.Length);

            foreach (var mac in parts)
            {
                RedisIppEntry? redisIppEntry = await this._cache.GetObject<RedisIppEntry>(mac);
                if (redisIppEntry != null)
                {
                    response.Add(new DmEntryResponse { Mac = mac, Data = redisIppEntry });
                }
            }
            
            return response;
        }

        /// <summary>
        /// Delete multiple cache entries by comma separated macs
        /// </summary>
        /// <param name="macs">comma separated macs to delete</param>
        /// <returns></returns>
        [HttpDelete("macs/{macs}")]
        public async Task DeleteMacs(string macs)
        {
            var parts = macs.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
              
            foreach (var mac in parts) 
            {
                bool deleted = await this._cache.KeyDeleteAsync(mac);
            }
        }
    }
}
