namespace DmAggregator.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpRouteConstants
    {
        /// <summary>
        /// With optional profile.
        /// </summary>
        public const string IPPPhoneMgrStatusBase = "c/{customerId}/p/{profileId}/rest/v1/ipphoneMgrStatus/";

        /// <summary>
        /// W/O optional profile
        /// Note - because the optional route parameter is not in the end, two different routes are required
        /// </summary>
        public const string IPPPhoneMgrStatusBaseWithoutProfile = "c/{customerId}/rest/v1/ipphoneMgrStatus/";

        /// <summary>
        /// Device manager base
        /// </summary>
        public const string DmBase = "/v1/topology/actions/";

        /// <summary>
        /// 
        /// </summary>
        public const string KeepAlivePath = "keep-alive";

        /// <summary>
        /// 
        /// </summary>
        public const string InitPath = "init";

        /// <summary>
        /// 
        /// </summary>
        public const string DmCmdPath = "deviceAsyncCmd";
    }
}
