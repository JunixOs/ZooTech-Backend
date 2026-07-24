using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using ZooTech.Infrastructure.Parametrization;

namespace ZooTech.Infrastructure.IntegrationTests;

public sealed class ConfigInvalidationSubscriberIntegrationTests
{
    [Fact]
    public async Task Subscriber_EvictsL1_OnPubSubMessage()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:42", "stale-value", TimeSpan.FromMinutes(5));

        var subMock = new Mock<ISubscriber>();
        Action<RedisChannel, RedisValue>? subscribedHandler = null;

        subMock.Setup(s => s.Subscribe(
                It.IsAny<RedisChannel>(),
                It.IsAny<Action<RedisChannel, RedisValue>>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisChannel, Action<RedisChannel, RedisValue>, CommandFlags>(
                (_, handler, _) => { subscribedHandler = handler; });

        var redisMock = new Mock<IConnectionMultiplexer>();
        redisMock.Setup(r => r.GetSubscriber(It.IsAny<object>()))
            .Returns(subMock.Object);

        var loggerMock = new Mock<ILogger<ConfigInvalidationSubscriber>>();
        var subscriber = new ConfigInvalidationSubscriber(
            redisMock.Object, memoryCache, loggerMock.Object);

        await subscriber.StartAsync(CancellationToken.None);

        subscribedHandler.Should().NotBeNull();

        subscribedHandler!(
            RedisChannel.Literal("tenant:config:invalidate"),
            "42");

        var l1Exists = memoryCache.TryGetValue("tenant:config:42", out _);
        l1Exists.Should().BeFalse();

        await subscriber.StopAsync(CancellationToken.None);
        subscriber.Dispose();
    }

    [Fact]
    public async Task Subscriber_InvalidMessage_DoesNothing()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:42", "stale-value", TimeSpan.FromMinutes(5));
        memoryCache.Set("tenant:config:99", "other-value", TimeSpan.FromMinutes(5));

        var subMock = new Mock<ISubscriber>();
        Action<RedisChannel, RedisValue>? subscribedHandler = null;

        subMock.Setup(s => s.Subscribe(
                It.IsAny<RedisChannel>(),
                It.IsAny<Action<RedisChannel, RedisValue>>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisChannel, Action<RedisChannel, RedisValue>, CommandFlags>(
                (_, handler, _) => { subscribedHandler = handler; });

        var redisMock = new Mock<IConnectionMultiplexer>();
        redisMock.Setup(r => r.GetSubscriber(It.IsAny<object>()))
            .Returns(subMock.Object);

        var loggerMock = new Mock<ILogger<ConfigInvalidationSubscriber>>();
        var subscriber = new ConfigInvalidationSubscriber(
            redisMock.Object, memoryCache, loggerMock.Object);

        await subscriber.StartAsync(CancellationToken.None);

        subscribedHandler.Should().NotBeNull();

        subscribedHandler!(
            RedisChannel.Literal("tenant:config:invalidate"),
            "not-a-number");

        var l1Exists42 = memoryCache.TryGetValue("tenant:config:42", out _);
        l1Exists42.Should().BeTrue();

        var l1Exists99 = memoryCache.TryGetValue("tenant:config:99", out _);
        l1Exists99.Should().BeTrue();

        await subscriber.StopAsync(CancellationToken.None);
        subscriber.Dispose();
    }

    [Fact]
    public async Task Subscriber_EvictsOnlyMatchingTenant()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:1", "tenant1-data", TimeSpan.FromMinutes(5));
        memoryCache.Set("tenant:config:2", "tenant2-data", TimeSpan.FromMinutes(5));

        var subMock = new Mock<ISubscriber>();
        Action<RedisChannel, RedisValue>? subscribedHandler = null;

        subMock.Setup(s => s.Subscribe(
                It.IsAny<RedisChannel>(),
                It.IsAny<Action<RedisChannel, RedisValue>>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisChannel, Action<RedisChannel, RedisValue>, CommandFlags>(
                (_, handler, _) => { subscribedHandler = handler; });

        var redisMock = new Mock<IConnectionMultiplexer>();
        redisMock.Setup(r => r.GetSubscriber(It.IsAny<object>()))
            .Returns(subMock.Object);

        var loggerMock = new Mock<ILogger<ConfigInvalidationSubscriber>>();
        var subscriber = new ConfigInvalidationSubscriber(
            redisMock.Object, memoryCache, loggerMock.Object);

        await subscriber.StartAsync(CancellationToken.None);

        subscribedHandler!(RedisChannel.Literal("tenant:config:invalidate"), "1");

        var tenant1Exists = memoryCache.TryGetValue("tenant:config:1", out _);
        tenant1Exists.Should().BeFalse();

        var tenant2Exists = memoryCache.TryGetValue("tenant:config:2", out _);
        tenant2Exists.Should().BeTrue();

        await subscriber.StopAsync(CancellationToken.None);
        subscriber.Dispose();
    }
}
