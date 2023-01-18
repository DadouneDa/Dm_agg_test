namespace DmAggregator.Hashing
{
    /// <summary>
    /// <see cref="HttpContext"/> hashing extensions
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Key of <see cref="HttpContext.Items"/> containing the byte array hash
        /// Note - byte array is used instead of string for performance.
        /// </summary>
        private const string _requestBodyHashBytesKey = "RequestBodyHashBytes";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static byte[]? GetRequestBodyHash(this HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue(_requestBodyHashBytesKey, out var bodyHash))
            {
                return (byte[])bodyHash!;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="hash"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetRequestBodyHash(this HttpContext httpContext, byte[] hash)
        {
            if (hash is null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            httpContext.Items[_requestBodyHashBytesKey] = hash;
        }

        /// <summary>
        /// Returns true if input hash equals current body hash
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool RequestBodyHashEquals(this HttpContext httpContext, byte[] hash)
        {
            if (hash is null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            if (!httpContext.Items.TryGetValue(_requestBodyHashBytesKey,out var hashObj))
            {
                return Enumerable.SequenceEqual(hash, (byte[])hashObj!);
            }

            return false;
        }
    }
}
