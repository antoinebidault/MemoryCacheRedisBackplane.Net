using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MemoryCacheRedisBackplane.Net
{


    public class MemoryCacheRedisBackplaneOptions
    {
        /// <summary>
        /// Redis connection string
        /// </summary>
        public string RedisConnectionString { get; set; }

        /// <summary>
        /// Nb retry if connection failed
        /// </summary>
        public int NbRetry { get; set; }

        /// <summary>
        /// Event subscription name
        /// </summary>
        public string EventSubscriptionName { get; set; } = "mem-cache-invalidation";

        /// <summary>
        /// Optional callback invoked when a cache invalidation event is received
        /// </summary>
        public Action<string> OnCacheInvalidation { get; set; }
    }
}
