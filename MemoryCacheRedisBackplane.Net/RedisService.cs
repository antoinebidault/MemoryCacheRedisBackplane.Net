using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MemoryCacheRedisBackplane.Net
{

    internal class RedisService : IDisposable
    {
        private ConnectionMultiplexer _redis;
        private int _retry = 1;
        private DateTime? _lastFailureDate = null;
        private Guid _originId = Guid.NewGuid();
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
                    string value = message.ToString();
                    if (value.StartsWith(_originId.ToString()))
                    {
                        return;
                    }

                    subscriptionCallback(RemoveOriginId(value));
                }
            }, CommandFlags.FireAndForget);
        }
        internal void Publish(string key)
        {
            if (_redis.IsConnected)
            {
                _sub.Publish(_options.EventSubscriptionName, AppendOriginId(key), CommandFlags.FireAndForget);
            }
        }

        public void Dispose()
        {
            _sub.Unsubscribe(_options.EventSubscriptionName);
            _redis.Close();
            _redis.Dispose();
        }

        /// <summary>
        /// Append the origin id to the key in order to identify the request origin
        /// </summary>
        private string AppendOriginId(string key)
        {
            return _originId.ToString() + "_" + key;
        }

        /// <summary>
        /// Remove the origin id from string
        /// </summary>
        private string RemoveOriginId(string key)
        {
            return key.Substring(key.IndexOf("_") + 1, key.Length - key.IndexOf("_") - 1);
        }
    }
}
