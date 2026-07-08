using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.Infrastructure.Caching
{
    public class GarnetCacheService : IAppCacheService
    {
        private readonly GarnetCacheConnection _garnetCacheConnection;
        private readonly ITenantContext _tenantContext;
        private readonly TimeSpan _defaultExpiration;
        private readonly ILogger<GarnetCacheService> _logger;

        public GarnetCacheService(
            GarnetCacheConnection garnetCacheConnection, 
            ITenantContext tenantContext,
            IConfiguration configuration,
            ILogger<GarnetCacheService> logger
        )
        {
            _garnetCacheConnection = garnetCacheConnection;
            _tenantContext = tenantContext;
            _logger = logger;
            
            var expirationTime = configuration["Garnet:ExpirationTime"];
            if (TimeSpan.TryParseExact(
                expirationTime,
                @"mm\:ss",
                CultureInfo.InvariantCulture,
                out var timeSpan))
            {
                _defaultExpiration = new TimeSpan(0, timeSpan.Minutes, timeSpan.Seconds);
            }
            else
            {
                _defaultExpiration = new TimeSpan(0, 3, 0);
            }
        }

        public async Task<(bool Found, T? Value)> TryGetAsync<T>(string key)
        {
            try
            {
                var multiplexer = _garnetCacheConnection.GetMultiplexer();
                if (!multiplexer.IsConnected)
                {
                    return (false, default);
                }

                key = $"{_tenantContext.TenantId}:{key}";
                var garnetDatabase = _garnetCacheConnection.GetDatabase();
                var json = await garnetDatabase.StringGetAsync(key);

                if (!json.HasValue)
                    return (false, default);

                var deserialized = JsonSerializer.Deserialize<T>((string)json!);
                return (true, deserialized);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "TryGetAsync failed for key: {Key}. Redis/Garnet might be unavailable.", key);
                return (false, default);
            }
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key, 
            Func<Task<T>> factory
        )
        {
            return await GetOrCreateAsync(key, factory, _defaultExpiration);
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan ttl
        )
        {
            try
            {
                var multiplexer = _garnetCacheConnection.GetMultiplexer();
                if (multiplexer.IsConnected)
                {
                    var (found, value) = await TryGetAsync<T>(key);
                    if (found)
                    {
                        return value!;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetOrCreateAsync read failed for key: {Key}.", key);
            }

            var result = await factory();

            try
            {
                var multiplexer = _garnetCacheConnection.GetMultiplexer();
                if (multiplexer.IsConnected)
                {
                    var fullKey = $"{_tenantContext.TenantId}:{key}";
                    var garnetDatabase = _garnetCacheConnection.GetDatabase();
                    var serialized = JsonSerializer.Serialize(result);
                    await garnetDatabase.StringSetAsync(fullKey, serialized, ttl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetOrCreateAsync write failed for key: {Key}.", key);
            }

            return result;
        }

        public async Task RemoveByKeyAsync(string key)
        {
            try
            {
                var multiplexer = _garnetCacheConnection.GetMultiplexer();
                if (!multiplexer.IsConnected)
                {
                    return;
                }

                key = $"{_tenantContext.TenantId}:{key}";
                var garnetDatabase = _garnetCacheConnection.GetDatabase();
                await garnetDatabase.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RemoveByKeyAsync failed for key: {Key}.", key);
            }
        }

        public async Task SaveAsync<T>(
            string key,
            T valueToCaching,
            TimeSpan ttl
        )
        {
            try
            {
                var multiplexer = _garnetCacheConnection.GetMultiplexer();
                if (!multiplexer.IsConnected)
                {
                    return;
                }

                key = $"{_tenantContext.TenantId}:{key}";
                var garnetDatabase = _garnetCacheConnection.GetDatabase();
                var serialized = JsonSerializer.Serialize(valueToCaching);
                await garnetDatabase.StringSetAsync(key, serialized, ttl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SaveAsync failed for key: {Key}.", key);
            }
        }
    }
}
