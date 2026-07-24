using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ZooTech.Infrastructure.Parametrization;

public sealed class ConfigInvalidationSubscriber : IHostedService, IDisposable
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ConfigInvalidationSubscriber> _logger;
    private ISubscriber? _subscriber;

    public ConfigInvalidationSubscriber(
        IConnectionMultiplexer redis,
        IMemoryCache memoryCache,
        ILogger<ConfigInvalidationSubscriber> logger)
    {
        _redis = redis;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _subscriber = _redis.GetSubscriber();
            _subscriber.Subscribe(RedisChannel.Literal("tenant:config:invalidate"), (_, message) =>
            {
                if (int.TryParse((string?)message, out var tenantId))
                {
                    _memoryCache.Remove(BuildKey(tenantId));
                }
            });
            _logger.LogInformation("Subscribed to tenant:config:invalidate channel");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to subscribe to Redis Pub/Sub. Multi-instance cache invalidation will not work.");
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _subscriber?.Unsubscribe(RedisChannel.Literal("tenant:config:invalidate"));
        }
        catch
        {
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        try
        {
            _subscriber?.Unsubscribe(RedisChannel.Literal("tenant:config:invalidate"));
        }
        catch
        {
        }
    }

    private static string BuildKey(int tenantId) => $"tenant:config:{tenantId}";
}
