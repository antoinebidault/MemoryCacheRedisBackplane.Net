using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryCacheRedisBackplane.Net
{
    /// <summary>
    /// Memory cache extensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add the memory cache redis invalidator to your app services
        /// This will register the IMemoryCacheInvalidator as a singleton in the whole application
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisConnectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemoryCacheRedisBackplane(this IServiceCollection services, string redisConnectionString)
        {
            return services.AddMemoryCacheRedisBackplane(redisConnectionString, c=> { });
        }

        /// <summary>
        /// Add the memory cache redis invalidator to your app services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisConnectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemoryCacheRedisBackplane(this IServiceCollection services, string redisConnectionString, Action<MemoryCacheRedisBackplaneOptions> configureOptions)
        {
            var options = new MemoryCacheRedisBackplaneOptions()
            {
                RedisConnectionString = redisConnectionString
            };

            if (configureOptions != null)
            {
                configureOptions.Invoke(options);
            }

            services.AddSingleton(options);

            return services.AddSingleton<IMemoryCacheInvalidator, MemoryCacheRedisBackplane>();
        }

        /// <summary>
        /// This will register an idle backplane that is not binded to the Redis server. 
        /// The IMemoryCacheInvalidator will only clear the memory cache
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemoryCacheDevelopmentBackplane(this IServiceCollection services)
        {
            return services.AddSingleton<IMemoryCacheInvalidator, MemoryCacheDevelopmentBackplane>();
        }
    }
}
