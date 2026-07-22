using System.Collections.Concurrent;

namespace ZooTech.Infrastructure.Caching.ConcurrentCache
{
    public sealed class ConcurrentCache<TKey, TValue> : IConcurrentCache<TKey, TValue>
        where TKey : notnull
    {
        private readonly ConcurrentDictionary<TKey, TValue> _dictionary = new();

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
        {
            return _dictionary.GetOrAdd(key, factory);
        }

        public bool TryGet(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value!);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return _dictionary.TryAdd(key, value);
        }

        public bool TryRemove(TKey key)
        {
            return _dictionary.TryRemove(key, out _);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }
    }
}