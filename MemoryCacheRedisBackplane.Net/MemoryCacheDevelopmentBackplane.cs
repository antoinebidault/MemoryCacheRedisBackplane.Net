using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MemoryCacheRedisBackplane.Net
{
    internal class MemoryCacheDevelopmentBackplane : IMemoryCacheInvalidator
    {
        private IMemoryCache _cache;

        public MemoryCacheDevelopmentBackplane(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task InvalidateAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

    }
}
