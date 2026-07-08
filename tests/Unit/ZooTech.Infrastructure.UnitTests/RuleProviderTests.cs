using Moq;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;
using ZooTech.Infrastructure.Parametrization.Rules;

namespace ZooTech.Infrastructure.UnitTests;

public sealed class RuleProviderTests
{
    [Fact]
    public async Task IsEnabledAsync_RuleInEnabledSet_ReturnsTrue()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<IRuleRepository>();

        cache.Setup(c => c.TryGetAsync<HashSet<string>>("rules:tenant:1"))
            .ReturnsAsync((true, new HashSet<string> { "AUTO_INVOICE" }));

        var provider = new RuleProvider(cache.Object, repository.Object);

        var result = await provider.IsEnabledAsync(1, new RuleCode("AUTO_INVOICE"));

        Assert.True(result);
    }

    [Fact]
    public async Task IsEnabledAsync_RuleNotInEnabledSet_ReturnsFalse()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<IRuleRepository>();

        cache.Setup(c => c.TryGetAsync<HashSet<string>>("rules:tenant:1"))
            .ReturnsAsync((true, new HashSet<string>()));

        var provider = new RuleProvider(cache.Object, repository.Object);

        var result = await provider.IsEnabledAsync(1, new RuleCode("NONEXISTENT"));

        Assert.False(result);
    }

    [Fact]
    public async Task IsEnabledAsync_CacheMiss_LoadsFromRepository()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<IRuleRepository>();

        cache.Setup(c => c.TryGetAsync<HashSet<string>>("rules:tenant:1"))
            .ReturnsAsync((false, (HashSet<string>?)null));

        repository.Setup(r => r.GetEnabledRuleCodesAsync(1))
            .ReturnsAsync(new HashSet<string> { "VACUNOS_ELIMINACION_CONDICIONADA" });

        cache.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<HashSet<string>>>>()))
            .Returns<string, Func<Task<HashSet<string>>>>((_, factory) => factory());

        var provider = new RuleProvider(cache.Object, repository.Object);

        var result = await provider.IsEnabledAsync(1, new RuleCode("VACUNOS_ELIMINACION_CONDICIONADA"));

        Assert.True(result);
        repository.Verify(r => r.GetEnabledRuleCodesAsync(1), Times.Once);
    }
}
