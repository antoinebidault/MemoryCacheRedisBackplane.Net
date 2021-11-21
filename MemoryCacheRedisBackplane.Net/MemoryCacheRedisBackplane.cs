using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MemoryCacheRedisBackplane.Net
{
    internal class MemoryCacheRedisBackplane : IMemoryCacheInvalidator
    {
        private RedisService _redis;
        private IMemoryCache _cache;

        public MemoryCacheRedisBackplane(MemoryCacheRedisBackplaneOptions options, IMemoryCache cache)
        {
            this._redis = new RedisService(options);
            _cache = cache;
            _redis.Subscribe(c =>
            {
                _cache.Remove(c);
            });
        }

        public Task InvalidateAsync(string key)
        {
            // _cache.Remove(key);
            return _redis.PublishAsync(key);
        }

    }
}
