using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MemoryCacheRedisBackplane.Net
{

    internal class RedisService
    {
        private ConnectionMultiplexer _redis;
        private int _retry = 1;
        private DateTime? _lastFailureDate = null;
        private Guid _originId;
        private MemoryCacheRedisBackplaneOptions _options;
        private ISubscriber _sub;

        internal RedisService(MemoryCacheRedisBackplaneOptions options)
        {
            this._originId = Guid.NewGuid();
            this._options = options;
            this.Connect(options.RedisConnectionString);
            _redis.ConnectionFailed += (s, e) =>
            {
                if (_retry < options.NbRetry || _lastFailureDate > DateTime.UtcNow.AddMinutes(5))
                {
                    Connect(options.RedisConnectionString);
                    _retry++;
                }
                _lastFailureDate = DateTime.UtcNow;
            };
        }

        internal void Connect(string cnx)
        {
            _redis = ConnectionMultiplexer.Connect(cnx);
            if (_redis.IsConnected)
            {
                _retry = 0;
            }
            _sub = _redis.GetSubscriber();
        }

        internal void Subscribe(Action<string> subscriptionCallback)
        {
            _sub.Subscribe(_options.EventSubscriptionName, (channel, message) =>
            {
                if (message.HasValue)
                {
                    subscriptionCallback(message.ToString());
                }
            });
        }
        internal Task PublishAsync(string key)
        {
            if (_redis.IsConnected)
            {
                return _sub.PublishAsync(_options.EventSubscriptionName, key);
            }
            return Task.CompletedTask;
        }

    }
}
