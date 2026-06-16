using Moq;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Configuration;
using ZooTech.Infrastructure.Parametrization.Settings;

namespace ZooTech.Infrastructure.UnitTests;

public sealed class SettingsProviderTests
{
    [Fact]
    public async Task GetAsync_CacheHit_ReturnsCachedValue()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<ISettingsRepository>();

        cache.Setup(c => c.TryGetAsync<SettingsSnapshot>("settings:tenant:1"))
            .ReturnsAsync((true, new SettingsSnapshot
            {
                Values = new() { { "max_login_attempts", "5" } }
            }));

        var provider = new SettingsProvider(cache.Object, repository.Object);
        var setting = new SettingDefinition<int>("max_login_attempts");

        var result = await provider.GetAsync(1, setting);

        Assert.Equal(5, result);
        repository.Verify(r => r.GetTenantSettingsAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetAsync_CacheMiss_LoadsFromRepository()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<ISettingsRepository>();

        cache.Setup(c => c.TryGetAsync<SettingsSnapshot>("settings:tenant:1"))
            .ReturnsAsync((false, (SettingsSnapshot?)null));

        repository.Setup(r => r.GetTenantSettingsAsync(1))
            .ReturnsAsync(new List<SettingValueDto>
            {
                new() { Code = "max_login_attempts", Value = "3" }
            });

        cache.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<SettingsSnapshot>>>()))
            .Returns<string, Func<Task<SettingsSnapshot>>>((_, factory) => factory());

        var provider = new SettingsProvider(cache.Object, repository.Object);
        var setting = new SettingDefinition<int>("max_login_attempts");

        var result = await provider.GetAsync(1, setting);

        Assert.Equal(3, result);
        repository.Verify(r => r.GetTenantSettingsAsync(1), Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_RemovesCacheEntry()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<ISettingsRepository>();

        var provider = new SettingsProvider(cache.Object, repository.Object);

        await provider.RefreshAsync(1);

        cache.Verify(c => c.RemoveByKeyAsync("settings:tenant:1"), Times.Once);
    }

    [Fact]
    public async Task GetAsync_SettingNotFound_ReturnsDefault()
    {
        var cache = new Mock<IAppCacheService>();
        var repository = new Mock<ISettingsRepository>();

        cache.Setup(c => c.TryGetAsync<SettingsSnapshot>("settings:tenant:1"))
            .ReturnsAsync((true, new SettingsSnapshot
            {
                Values = new() { { "other_key", "42" } }
            }));

        var provider = new SettingsProvider(cache.Object, repository.Object);
        var setting = new SettingDefinition<int>("nonexistent");

        var result = await provider.GetAsync(1, setting);

        Assert.Equal(0, result);
    }
}
