using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Caching;

namespace ZooTech.Infrastructure.Caching
{
    public class GarnetCacheService : IAppCacheService
    {
        private readonly GarnetCacheConnection _garnetCacheConnection;
        private readonly TimeSpan _defaultExpiration;

        public GarnetCacheService(
            GarnetCacheConnection garnetCacheConnection, 
            IConfiguration configuration
        )
        {
            _garnetCacheConnection = garnetCacheConnection;
            
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
            var garnetDatabase = _garnetCacheConnection.GetDatabase();

            var json = await garnetDatabase.StringGetAsync(key);

            if (!json.HasValue)
                return (false, default);

            try
            {
                return (
                    true,
                    JsonSerializer.Deserialize<T>((string)json!)
                );
            }
            catch (JsonException)
            {
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
            var (found , value) = await TryGetAsync<T>(key);

            if(found)
            {
                return value!;
            }

            var garnetDatabase = _garnetCacheConnection.GetDatabase();

            var result = await factory();

            var serialized = JsonSerializer.Serialize(result);

            await garnetDatabase.StringSetAsync(
                key,
                serialized, 
                ttl
            );

            return result;
        }

        public async Task RemoveByKeyAsync(string key)
        {
            var garnetDatabase = _garnetCacheConnection.GetDatabase();

            await garnetDatabase.KeyDeleteAsync(key);
        }
    }
}
