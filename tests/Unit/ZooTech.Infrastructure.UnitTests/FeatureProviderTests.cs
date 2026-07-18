using Moq;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;
using ZooTech.Infrastructure.Parametrization.Features;

namespace ZooTech.Infrastructure.UnitTests;

public sealed class FeatureProviderTests
{
    [Fact]
    public async Task IsEnabledAsync_FeatureInEnabledSet_ReturnsTrue()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<IFeatureRepository>();

        cache.Setup(c => c.TryGetAsync<HashSet<string>>("features:tenant:1"))
            .ReturnsAsync((true, new HashSet<string> { "MODULE_INVENTORY", "MODULE_BILLING" }));

        var provider = new FeatureProvider(cache.Object, repository.Object);

        var result = await provider.IsEnabledAsync(1, new FeatureCode("MODULE_INVENTORY"));

        Assert.True(result);
    }

    [Fact]
    public async Task IsEnabledAsync_FeatureNotInEnabledSet_ReturnsFalse()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<IFeatureRepository>();

        cache.Setup(c => c.TryGetAsync<HashSet<string>>("features:tenant:1"))
            .ReturnsAsync((true, new HashSet<string> { "MODULE_BILLING" }));

        var provider = new FeatureProvider(cache.Object, repository.Object);

        var result = await provider.IsEnabledAsync(1, new FeatureCode("MODULE_INVENTORY"));

        Assert.False(result);
    }

    [Fact]
    public async Task IsEnabledAsync_CacheMiss_LoadsFromRepository()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<IFeatureRepository>();

        cache.Setup(c => c.TryGetAsync<HashSet<string>>("features:tenant:1"))
            .ReturnsAsync((false, (HashSet<string>?)null));

        repository.Setup(r => r.GetEnabledFeatureCodesAsync(1))
            .ReturnsAsync(new HashSet<string> { "MODULE_VACUNOS" });

        cache.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<HashSet<string>>>>()))
            .Returns<string, Func<Task<HashSet<string>>>>((_, factory) => factory());

        var provider = new FeatureProvider(cache.Object, repository.Object);

        var result = await provider.IsEnabledAsync(1, new FeatureCode("MODULE_VACUNOS"));

        Assert.True(result);
        repository.Verify(r => r.GetEnabledFeatureCodesAsync(1), Times.Once);
    }
}
