namespace DmAggregator.Services.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedisCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T?> GetObject<T>(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        Task<T?> GetObjectSetExpiry<T>(string key, TimeSpan ttl);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetVersion();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> KeyDeleteAsync(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        Task<bool> KeyExpireAsync(string key, TimeSpan ttl);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TimeSpan?> KeyTimeToLiveAsync(string key);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task<bool> SetObject<T>(string key, T obj);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task<bool> SetObject<T>(string key, T obj, TimeSpan expiry);
    }
}