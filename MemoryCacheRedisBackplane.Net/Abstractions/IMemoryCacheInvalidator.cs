using System.Threading.Tasks;

namespace MemoryCacheRedisBackplane.Net
{
    /// <summary>
    /// Cache invalidation service (used to invalidate the cache through multiple instances)
    /// </summary>
    public interface IMemoryCacheInvalidator
    {
        /// <summary>
        /// Clear the cache key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task InvalidateAsync(string key);
    }
}
