using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MemoryCacheRedisBackplane.Net
{
    internal class MemoryCacheRedisBackplane : IMemoryCacheInvalidator, IDisposable
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

        public async Task InvalidateAsync(string key)
        {
            await Task.Run(() => Invalidate(key));
        }

        public void Invalidate(string key)
        {
            _cache.Remove(key);
            _redis.Publish(key);
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
