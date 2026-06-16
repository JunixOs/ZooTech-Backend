using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Caching;

namespace ZooTech.Infrastructure.Caching
{
    public class RedisCacheService : IAppCacheService
    {
        private readonly RedisCacheConnection _redisCacheConnection;
        private readonly TimeSpan _expirationTimeSpan;

        public RedisCacheService(
            RedisCacheConnection redisCacheConnection , 
            IConfiguration configuration
        )
        {
            _redisCacheConnection = redisCacheConnection;
            
            var expirationTime = configuration["Redis:ExpirationTime"];
            if(TimeSpan.TryParseExact(
                expirationTime,
                @"mm\:ss",
                CultureInfo.InvariantCulture,
                out var timeSpan))
            {
                _expirationTimeSpan = new TimeSpan(0, timeSpan.Minutes , timeSpan.Seconds);
            }
            else
            {
                _expirationTimeSpan = new TimeSpan(0, 3 , 0);
            }

            
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key, 
            Func<Task<T>> factory
        )
        {
            return await GetOrCreateAsync(key, factory, _expirationTimeSpan);
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan ttl
        )
        {
            var redisDatabase = _redisCacheConnection.GetDatabase();

            var cachedValue = await redisDatabase.StringGetAsync(key);

            if (cachedValue.HasValue)
            {
                var bytes = (byte[])cachedValue!;

                var cachedResult =  JsonSerializer.Deserialize<T>(
                    bytes
                )!;

                if (cachedResult is not null)
                {
                    return cachedResult;
                }
            }

            var result = await factory();

            var serialized = JsonSerializer.Serialize(result);

            await redisDatabase.StringSetAsync(
                key ,
                serialized , 
                ttl
            );

            return result;
        }

        public async Task<(bool Found, T? Value)> TryGetAsync<T>(string key)
        {
            var redisDatabase = _redisCacheConnection.GetDatabase();

            var cachedValue = await redisDatabase.StringGetAsync(key);

            if (!cachedValue.IsNullOrEmpty)
            {
                try
                {
                    var value = JsonSerializer.Deserialize<T>((string)cachedValue!);
                    return (true, value);
                }
                catch (JsonException)
                {
                    return (false, default);
                }
            }

            return (false, default);
        }

        public async Task RemoveByKeyAsync(string key)
        {
            var redisDatabase = _redisCacheConnection.GetDatabase();
            await redisDatabase.KeyDeleteAsync(key);
        }
    }
}