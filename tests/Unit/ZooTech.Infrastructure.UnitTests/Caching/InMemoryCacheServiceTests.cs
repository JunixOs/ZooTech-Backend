using ZooTech.Infrastructure.Caching;

namespace ZooTech.Infrastructure.UnitTests.Caching;

public sealed class InMemoryCacheServiceTests
{
    [Fact]
    public async Task RemoveByPrefixAsync_RemovesOnlyMatchingKeys()
    {
        var cache = new InMemoryCacheService();
        var prefix = $"test:{Guid.NewGuid():N}:";
        var matchingKey = $"{prefix}list";
        var otherKey = $"other:{Guid.NewGuid():N}";

        await cache.SaveAsync(matchingKey, "remove", TimeSpan.FromMinutes(1));
        await cache.SaveAsync(otherKey, "keep", TimeSpan.FromMinutes(1));

        await cache.RemoveByPrefixAsync(prefix);

        var removed = await cache.TryGetAsync<string>(matchingKey);
        var retained = await cache.TryGetAsync<string>(otherKey);

        Assert.False(removed.Found);
        Assert.True(retained.Found);
        Assert.Equal("keep", retained.Value);

        await cache.RemoveByKeyAsync(otherKey);
    }
}
