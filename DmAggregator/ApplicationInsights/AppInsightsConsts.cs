namespace DmAggregator.ApplicationInsights
{
    /// <summary>
    /// 
    /// </summary>
    public static class AppInsightsConsts
    {
        // Custom Events

        /// <summary>
        /// 
        /// </summary>
        public const string IppKeepAliveEvent = "ippKeepAlive";


        // Custom Properties/values

        /// <summary>
        /// 
        /// </summary>
        public const string MacProperty = "mac";

        /// <summary>
        /// 
        /// </summary>
        public const string CustomerStatusProperty = "customerStatus";

        /// <summary>
        /// 
        /// </summary>
        public const string NoCustomersValue = "noCustomers";

        /// <summary>
        /// 
        /// </summary>
        public const string NotFoundValue = "notFound";

        /// <summary>
        /// 
        /// </summary>
        public const string ValidValue = "valid";
        
        /// <summary>
        /// 
        /// </summary>
        public const string RejectedMacProperty= "rejectedMac";
        
        /// <summary>
        /// 
        /// </summary>
        public const string CacheStatusProperty= "cacheStatus";

        /// <summary>
        /// 
        /// </summary>
        public const string CacheChangedValue = "cacheChanged";

        /// <summary>
        /// 
        /// </summary>
        public const string CacheHitValue = "cacheHit";

        /// <summary>
        /// 
        /// </summary>
        public const string ActionsProperty = "actions";

        /// <summary>
        /// 
        /// </summary>
        public const string TtlProperty = "ttl";

        // Dependencies

        /// <summary>
        /// 
        /// </summary>
        public const string DepRedisGetObject = "getObject";

        /// <summary>
        /// 
        /// </summary>
        public const string DepRedisGetObjectSetExpiry = "getObjectSetExpiry";

        /// <summary>
        /// 
        /// </summary>
        public const string DepRedisSetObject= "setObject";

        /// <summary>
        /// 
        /// </summary>
        public const string DepRedisKeyDelete = "keyDelete";

        /// <summary>
        /// 
        /// </summary>
        public const string DepRedisKeyExpire = "keyExpire";

        /// <summary>
        /// 
        /// </summary>
        public const string DepRedisKeyTtl = "keyTtl";
    }
}
