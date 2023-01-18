using DmAggregator.Models;
using DmAggregator.Services.Redis;
using Microsoft.AspNetCore.Mvc;

namespace DmAggregator.Controllers
{
    /// <summary>
    /// Redis Controller
    /// </summary>
    [ApiController]
    [Route("api/redis")]
    public class RedisController : ControllerBase
    {
        private readonly IRedisCacheService _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        public RedisController(IRedisCacheService cache)
        {
            this._cache = cache;
        }

        /// <summary>
        /// Get servers version
        /// </summary>
        /// <returns></returns>
        [HttpGet("version")]
        public Dictionary<string,string> GetVersion()
        {
            return this._cache.GetVersion();
        }

        /// <summary>
        /// Get Redis IPP entry
        /// </summary>
        /// <param name="mac"></param>
        /// <returns>value and mac TTL</returns>
        [HttpGet("{mac}")]
        public async Task<IActionResult> Get(string mac)
        {
            RedisIppEntry? redisIppEntry = await this._cache.GetObject<RedisIppEntry>(mac);
            if (redisIppEntry == null)
            {
                return NotFound();
            }
            else
            {
                TimeSpan? ttl = await this._cache.KeyTimeToLiveAsync(mac);
                return Ok(new
                {
                    Value = redisIppEntry,
                    TTL = ttl.HasValue ? Math.Floor(ttl.Value.TotalSeconds).ToString() : null,
                });
            }
        }

        /// <summary>
        /// Delete a Redis entry
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        [HttpDelete("{mac}")]
        public async Task<IActionResult> Delete(string mac)
        {
            bool removed = await this._cache.KeyDeleteAsync(mac);
            return removed ? Ok() : NotFound();
        }
    }
}
