using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;
using ZooTech.Infrastructure.Parametrization;

namespace ZooTech.Infrastructure.IntegrationTests;

public sealed class TenantConfigurationProviderIntegrationTests
{
    private static TenantConfiguration CreateSampleConfig(int tenantId = 1) => new()
    {
        TenantId = tenantId,
        Settings = new() { { "MAX_LOGIN_ATTEMPTS", "5" } },
        EnabledFeatures = new() { "MODULE_VACUNOS" },
        EnabledRules = new() { "VACUNOS_ELIMINACION_CONDICIONADA" },
        LoadedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task Provider_EndToEnd_CacheThenDB()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var cacheMock = new Mock<IAppCacheService>();
        var repoMock = new Mock<ITenantConfigurationRepository>();

        // Phase 1: Cold start — cache empty, must load from repo
        cacheMock.Setup(c => c.TryGetAsync<TenantConfiguration>("tenant:config:1"))
            .ReturnsAsync((false, (TenantConfiguration?)null));
        cacheMock.Setup(c => c.GetOrCreateAsync(
                "tenant:config:1",
                It.IsAny<Func<Task<TenantConfiguration>>>(),
                TimeSpan.FromMinutes(5)))
            .Returns<string, Func<Task<TenantConfiguration>>, TimeSpan>(
                (_, factory, _) => factory());
        repoMock.Setup(r => r.LoadTenantConfigAsync(1))
            .ReturnsAsync(sampleConfig);

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object);

        var coldResult = await provider.GetSettingAsync(
            1, new SettingDefinition<int>("MAX_LOGIN_ATTEMPTS"));
        coldResult.Should().Be(5);
        repoMock.Verify(r => r.LoadTenantConfigAsync(1), Times.Once);

        // Phase 2: Hot — L1 populated, no L2 or DB calls
        repoMock.Invocations.Clear();
        cacheMock.Invocations.Clear();

        var hotResult = await provider.GetSettingAsync(
            1, new SettingDefinition<int>("MAX_LOGIN_ATTEMPTS"));
        hotResult.Should().Be(5);
        repoMock.Verify(r => r.LoadTenantConfigAsync(It.IsAny<int>()), Times.Never);

        // Phase 3: Invalidate — clears both L1 and L2
        await provider.InvalidateTenantAsync(1);

        var l1Exists = memoryCache.TryGetValue("tenant:config:1", out _);
        l1Exists.Should().BeFalse();
        cacheMock.Verify(c => c.RemoveByKeyAsync("tenant:config:1"), Times.Once);

        // Phase 4: Cold again — must reload from repo
        repoMock.Invocations.Clear();
        cacheMock.Invocations.Clear();

        cacheMock.Setup(c => c.TryGetAsync<TenantConfiguration>("tenant:config:1"))
            .ReturnsAsync((false, (TenantConfiguration?)null));
        cacheMock.Setup(c => c.GetOrCreateAsync(
                "tenant:config:1",
                It.IsAny<Func<Task<TenantConfiguration>>>(),
                TimeSpan.FromMinutes(5)))
            .Returns<string, Func<Task<TenantConfiguration>>, TimeSpan>(
                (_, factory, _) => factory());
        repoMock.Setup(r => r.LoadTenantConfigAsync(1))
            .ReturnsAsync(sampleConfig);

        var coldAgainResult = await provider.GetSettingAsync(
            1, new SettingDefinition<int>("MAX_LOGIN_ATTEMPTS"));
        coldAgainResult.Should().Be(5);
    }

    [Fact]
    public async Task L1_PopulatedFromL2_OnCacheHit()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var cacheMock = new Mock<IAppCacheService>();
        cacheMock.Setup(c => c.TryGetAsync<TenantConfiguration>("tenant:config:1"))
            .ReturnsAsync((true, sampleConfig));

        var repoMock = new Mock<ITenantConfigurationRepository>();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object);

        var result1 = await provider.GetSettingAsync(
            1, new SettingDefinition<int>("MAX_LOGIN_ATTEMPTS"));
        result1.Should().Be(5);

        var l1Exists = memoryCache.TryGetValue("tenant:config:1", out TenantConfiguration? l1Cached);
        l1Exists.Should().BeTrue();
        l1Cached.Should().NotBeNull();
    }
}
