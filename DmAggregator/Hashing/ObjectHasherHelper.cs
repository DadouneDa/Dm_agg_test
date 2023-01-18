using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace DmAggregator.Hashing
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectHasherHelper
    {
        private static readonly SHA1 s_hasher = SHA1.Create();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ComputeHash<TObj>(TObj obj)
        {
            var objBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);
            using (var hasher = SHA1.Create())
            {
                var hash = hasher.ComputeHash(objBytes);
                return hash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ComputeHashReusable<TObj>(TObj obj)
        {
            var objBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);
            lock (s_hasher)
            {
                var hash = s_hasher.ComputeHash(objBytes);
                return hash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static byte[] ComputeHashReusable<TObj>(TObj obj, IEnumerable<PropertyInfo> ignoreProperties)
        {
            var restoreInfo = ClearProperties(obj, ignoreProperties);

            var hash = ComputeHashReusable(obj);

            RestoreProperties(obj, restoreInfo);

            return hash;
        }

        private static IEnumerable<(PropertyInfo propInfo, object? value)> ClearProperties<TObj>(TObj obj, IEnumerable<PropertyInfo> ignoreProperties)
        {
            List<(PropertyInfo props, object? value)> restore = new List<(PropertyInfo props, object? value)>();
            foreach (var propertyInfo in ignoreProperties)
            {
                restore.Add((propertyInfo, propertyInfo.GetValue(obj)));
                propertyInfo.SetValue(obj, null);
            }
            return restore;
        }

        private static void RestoreProperties<TObj>(TObj obj, IEnumerable<(PropertyInfo propInfo, object? value)> restore)
        {
            foreach (var item in restore)
            {
                item.propInfo.SetValue(obj, item.value);
            }
        }
    }
}
