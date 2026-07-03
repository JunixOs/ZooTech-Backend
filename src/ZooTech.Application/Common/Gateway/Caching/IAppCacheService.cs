namespace ZooTech.Application.Common.Gateway.Caching
{
    public interface IAppCacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl);
        Task<(bool Found, T? Value)> TryGetAsync<T>(string key);
        Task RemoveByKeyAsync(string key);
        Task SaveAsync<T>(string key, T valueToCaching, TimeSpan ttl);
    }
}