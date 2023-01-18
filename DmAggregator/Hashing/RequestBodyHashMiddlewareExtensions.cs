namespace DmAggregator.Hashing
{
    /// <summary>
    /// 
    /// </summary>
    public static class RequestBodyHashMiddlewareExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestBodyHash(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestBodyHashMiddleware>();
        }
    }
}
