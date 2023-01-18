namespace DmAggregator.Hashing
{
    /// <summary>
    /// Calculates Request body hash and sets result in <see cref="HttpContext.Items"/> 
    /// </summary>
    public class RequestBodyHashMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public RequestBodyHashMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var origBodyStream = context.Request.Body;

            using (var newBodyStream = new StreamReadHasher(context, origBodyStream))
            {
                context.Request.Body = newBodyStream;

                await this._next(context);
            }
        }

    }
}
