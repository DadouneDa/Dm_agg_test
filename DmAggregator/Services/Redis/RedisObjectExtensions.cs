using StackExchange.Redis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DmAggregator.Services.Redis
{
    /// <summary>
    /// Extensions to REDIS strings as JSON objects
    /// </summary>
    public static class RedisObjectExtensions
    {
        private static readonly JsonSerializerOptions s_jsonSerializerOptions =
            new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(),
                }
            };

        /// <summary>
        /// Sets object as JSON string value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static async Task<bool> SetObject<T>(this IDatabase db, string key, T value, TimeSpan? expiry = null)
        {
            string json = JsonSerializer.Serialize(value, s_jsonSerializerOptions);
            return await db.StringSetAsync(key, json, expiry);
        }

        /// <summary>
        /// Returns object from JSON string value 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T?> GetObject<T>(this IDatabase db, string key)
        {
            string? json = await db.StringGetAsync(key);

            if (!string.IsNullOrEmpty(json))
            {
                T? obj = JsonSerializer.Deserialize<T>(json, s_jsonSerializerOptions);
                return obj;
            }
            return default;
        }

        /// <summary>
        /// Returns object from JSON string value and sets key expiration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static async Task<T?> GetObjectSetExpiry<T>(this IDatabase db, string key, TimeSpan expiry)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            if (expiry < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(expiry));
            }

            // Use script to get and set expiry due to Azure not supporting GETEX
            string? json = await db.ScriptStringGetSetExpiry(key, expiry);

            if (!string.IsNullOrEmpty(json))
            {
                T? obj = JsonSerializer.Deserialize<T>(json, s_jsonSerializerOptions);
                return obj;
            }
            return default;
        }
    }
}
