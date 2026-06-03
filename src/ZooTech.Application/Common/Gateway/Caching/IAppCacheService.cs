namespace ZooTech.Application.Common.Gateway.Caching
{
    public interface IAppCacheService
    {
        Task<T> GetOrCreateAsync<T>(string key , Func<Task<T>> factory);
    }
}