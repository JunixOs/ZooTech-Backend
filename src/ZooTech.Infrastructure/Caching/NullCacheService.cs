namespace ZooTech.Infrastructure.Caching
{
    /// <summary>
    /// Cache de desarrollo: no hace nada. Evita depender de Redis/Garnet en localhost.
    /// </summary>
    public class NullCacheService : ZooTech.Application.Common.Gateway.Caching.IAppCacheService
    {
        public Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
            => factory();

        public Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl)
            => factory();

        public Task<(bool Found, T? Value)> TryGetAsync<T>(string key)
            => Task.FromResult<(bool, T?)>((false, default));

        public Task RemoveByKeyAsync(string key) => Task.CompletedTask;

        public Task SaveAsync<T>(string key, T valueToCaching, TimeSpan ttl) => Task.CompletedTask;
    }
}
