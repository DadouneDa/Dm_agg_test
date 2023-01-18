using StackExchange.Redis;

namespace DmAggregator.Services.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDatabaseScriptExtensions
    {
        // This is the Lua script implementation of GETEX
        private static readonly LuaScript s_scriptGetSetExpiry =
            LuaScript.Prepare("local val=redis.call('get',@key); if val then redis.call('expire', @key,@seconds) end; return val");

        /// <summary>
        /// Script implementation of Redis GETEX - Get the value of key and optionally set its expiration. 
        /// GETEX is implemented on Redis since 6.2.0, and therefore not available on Azure yet.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static async Task<RedisValue> ScriptStringGetSetExpiry(this IDatabase db, RedisKey key, TimeSpan expiry)
        {
            if (expiry <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(expiry));
            }
            RedisValue scriptResult = (RedisValue)await db.ScriptEvaluateAsync(s_scriptGetSetExpiry, new { key, seconds = (int)expiry.TotalSeconds });
            return scriptResult;
        }
    }
}
