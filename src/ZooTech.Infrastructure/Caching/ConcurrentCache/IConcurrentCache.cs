namespace ZooTech.Infrastructure.Caching.ConcurrentCache
{
    public interface IConcurrentCache<TKey, TValue>
        where TKey : notnull
    {
        TValue GetOrAdd(TKey key, Func<TKey, TValue> factory);

        bool TryGet(TKey key, out TValue value);

        bool TryAdd(TKey key, TValue value);

        bool TryRemove(TKey key);

        void Clear();
    }
}