using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Caching;

namespace ZooTech.Infrastructure.Caching
{
    public class GarnetCacheService : IAppCacheService
    {
        private readonly GarnetCacheConnection _garnetCacheConnection;
        private readonly TimeSpan _expirationTimeSpan;

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
                _expirationTimeSpan = new TimeSpan(0, timeSpan.Minutes, timeSpan.Seconds);
            }
            else
            {
                _expirationTimeSpan = new TimeSpan(0, 3, 0);
            }
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key, 
            Func<Task<T>> factory
        )
        {
            var garnetDatabase = _garnetCacheConnection.GetDatabase();

            var cachedValue = await garnetDatabase.StringGetAsync(key);

            if (cachedValue.HasValue)
            {
                byte[] bytes = cachedValue!;

                var cachedResult = JsonSerializer.Deserialize<T>(
                    bytes
                )!;

                if (cachedResult is not null)
                {
                    return cachedResult;
                }
            }

            var result = await factory();

            var serialized = JsonSerializer.Serialize(result);

            await garnetDatabase.StringSetAsync(
                key,
                serialized, 
                _expirationTimeSpan
            );

            return result;
        }
    }
}
