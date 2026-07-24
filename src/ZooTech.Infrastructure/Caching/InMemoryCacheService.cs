using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ZooTech.Application.Common.Gateway.Caching;

namespace ZooTech.Infrastructure.Caching
{
    public class InMemoryCacheService : IAppCacheService
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new();

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            var newValue = await factory();
            if (newValue != null)
            {
                _cache[key] = newValue;
            }
            return newValue;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl)
        {
            return await GetOrCreateAsync(key, factory);
        }

        public Task<(bool Found, T? Value)> TryGetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return Task.FromResult((true, (T?)value));
            }
            return Task.FromResult((false, default(T)));
        }

        public Task RemoveByKeyAsync(string key)
        {
            _cache.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public Task SaveAsync<T>(string key, T valueToCaching, TimeSpan ttl)
        {
            if (valueToCaching != null)
            {
                _cache[key] = valueToCaching;
            }
            return Task.CompletedTask;
        }
    }
}
