namespace ZooTech.Application.Common.Caching;

public interface IEvictCacheRequest
{
    string[] GetCachePrefixesToEvict();
}
