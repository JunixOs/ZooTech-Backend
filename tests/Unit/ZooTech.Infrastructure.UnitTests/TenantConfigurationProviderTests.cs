using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;
using ZooTech.Infrastructure.Parametrization;

namespace ZooTech.Infrastructure.UnitTests;

public sealed class TenantConfigurationProviderTests
{
    private static TenantConfiguration CreateSampleConfig(int tenantId = 1) => new()
    {
        TenantId = tenantId,
        Settings = new()
        {
            { "MAX_LOGIN_ATTEMPTS", "5" },
            { "SESSION_TIMEOUT_MINUTES", "30" },
            { "FEATURE_X_ENABLED", "true" },
            { "WELCOME_MESSAGE", "Hello" }
        },
        EnabledFeatures = new() { "MODULE_VACUNOS", "MODULE_REPORTES" },
        EnabledRules = new() { "VACUNOS_ELIMINACION_CONDICIONADA" },
        LoadedAt = DateTime.UtcNow
    };

    private static Mock<ITenantContext> CreateTenantContextMock(int tenantId = 1)
    {
        var tenantContextMock = new Mock<ITenantContext>();
        tenantContextMock.Setup(t => t.TenantId).Returns(tenantId);
        return tenantContextMock;
    }

    [Fact]
    public async Task GetConfigAsync_L1Hit_SkipsL2AndDb()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:1", sampleConfig, TimeSpan.FromMinutes(5));

        var cacheMock = new Mock<IAppCacheService>();
        var repoMock = new Mock<ITenantConfigurationRepository>();
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        var result = await provider.GetSettingAsync(
            new SettingDefinition<int>("MAX_LOGIN_ATTEMPTS"));

        result.Should().Be(5);
        cacheMock.Verify(
            c => c.TryGetAsync<TenantConfiguration>(It.IsAny<string>()), Times.Never);
        repoMock.Verify(
            r => r.LoadTenantConfigAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetConfigAsync_L1Miss_L2Hit_PopulatesL1()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var cacheMock = new Mock<IAppCacheService>();
        cacheMock.Setup(c => c.TryGetAsync<TenantConfiguration>("tenant:config:1"))
            .ReturnsAsync((true, sampleConfig));

        var repoMock = new Mock<ITenantConfigurationRepository>();
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        var result = await provider.GetSettingAsync(
            new SettingDefinition<int>("MAX_LOGIN_ATTEMPTS"));

        result.Should().Be(5);
        repoMock.Verify(r => r.LoadTenantConfigAsync(It.IsAny<int>()), Times.Never);

        var l1Exists = memoryCache.TryGetValue("tenant:config:1", out TenantConfiguration? cached);
        l1Exists.Should().BeTrue();
        cached.Should().NotBeNull();
        cached!.Settings.Should().ContainKey("MAX_LOGIN_ATTEMPTS");
    }

    [Fact]
    public async Task GetConfigAsync_FullMiss_LoadsFromRepo()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var cacheMock = new Mock<IAppCacheService>();
        cacheMock.Setup(c => c.TryGetAsync<TenantConfiguration>("tenant:config:1"))
            .ReturnsAsync((false, (TenantConfiguration?)null));
        cacheMock.Setup(c => c.GetOrCreateAsync(
                "tenant:config:1",
                It.IsAny<Func<Task<TenantConfiguration>>>(),
                TimeSpan.FromMinutes(5)))
            .Returns<string, Func<Task<TenantConfiguration>>, TimeSpan>(
                (_, factory, _) => factory());

        var repoMock = new Mock<ITenantConfigurationRepository>();
        repoMock.Setup(r => r.LoadTenantConfigAsync(1))
            .ReturnsAsync(sampleConfig);
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        var result = await provider.GetSettingAsync(
            new SettingDefinition<int>("MAX_LOGIN_ATTEMPTS"));

        result.Should().Be(5);
        repoMock.Verify(r => r.LoadTenantConfigAsync(1), Times.Once);

        var l1Exists = memoryCache.TryGetValue("tenant:config:1", out TenantConfiguration? l1Cached);
        l1Exists.Should().BeTrue();
        l1Cached.Should().NotBeNull();

        cacheMock.Verify(c => c.GetOrCreateAsync(
            "tenant:config:1",
            It.IsAny<Func<Task<TenantConfiguration>>>(),
            TimeSpan.FromMinutes(5)), Times.Once);
    }

    [Fact]
    public async Task InvalidateTenantAsync_EvictsBothLayers()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:1", sampleConfig, TimeSpan.FromMinutes(5));

        var cacheMock = new Mock<IAppCacheService>();
        var repoMock = new Mock<ITenantConfigurationRepository>();
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        await provider.InvalidateTenantAsync();

        var l1Exists = memoryCache.TryGetValue("tenant:config:1", out _);
        l1Exists.Should().BeFalse();

        cacheMock.Verify(
            c => c.RemoveByKeyAsync("tenant:config:1"), Times.Once);
    }

    [Fact]
    public async Task GetSettingAsync_SettingFound_ReturnsTypedValue()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:1", sampleConfig, TimeSpan.FromMinutes(5));

        var cacheMock = new Mock<IAppCacheService>();
        var repoMock = new Mock<ITenantConfigurationRepository>();
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        var intResult = await provider.GetSettingAsync(
            new SettingDefinition<int>("MAX_LOGIN_ATTEMPTS"));
        intResult.Should().Be(5);

        var boolResult = await provider.GetSettingAsync(
            new SettingDefinition<bool>("FEATURE_X_ENABLED"));
        boolResult.Should().BeTrue();

        var stringResult = await provider.GetSettingAsync(
            new SettingDefinition<string>("WELCOME_MESSAGE"));
        stringResult.Should().Be("Hello");
    }

    [Fact]
    public async Task GetSettingAsync_SettingNotFound_ReturnsDefault()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:1", sampleConfig, TimeSpan.FromMinutes(5));

        var cacheMock = new Mock<IAppCacheService>();
        var repoMock = new Mock<ITenantConfigurationRepository>();
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        var intResult = await provider.GetSettingAsync(
            new SettingDefinition<int>("NONEXISTENT_INT"));
        intResult.Should().Be(0);

        var boolResult = await provider.GetSettingAsync(
            new SettingDefinition<bool>("NONEXISTENT_BOOL"));
        boolResult.Should().BeFalse();
    }

    [Fact]
    public async Task IsFeatureEnabledAsync_FeatureInSet_ReturnsTrue()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:1", sampleConfig, TimeSpan.FromMinutes(5));

        var cacheMock = new Mock<IAppCacheService>();
        var repoMock = new Mock<ITenantConfigurationRepository>();
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        var result = await provider.IsFeatureEnabledAsync(
            new FeatureCode("MODULE_VACUNOS"));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFeatureEnabledAsync_FeatureNotInSet_ReturnsFalse()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:1", sampleConfig, TimeSpan.FromMinutes(5));

        var cacheMock = new Mock<IAppCacheService>();
        var repoMock = new Mock<ITenantConfigurationRepository>();
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        var result = await provider.IsFeatureEnabledAsync(
            new FeatureCode("MODULE_INVENTORY"));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsRuleEnabledAsync_RuleInSet_ReturnsTrue()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:1", sampleConfig, TimeSpan.FromMinutes(5));

        var cacheMock = new Mock<IAppCacheService>();
        var repoMock = new Mock<ITenantConfigurationRepository>();
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        var result = await provider.IsRuleEnabledAsync(
            new RuleCode("VACUNOS_ELIMINACION_CONDICIONADA"));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsRuleEnabledAsync_RuleNotInSet_ReturnsFalse()
    {
        var sampleConfig = CreateSampleConfig();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("tenant:config:1", sampleConfig, TimeSpan.FromMinutes(5));

        var cacheMock = new Mock<IAppCacheService>();
        var repoMock = new Mock<ITenantConfigurationRepository>();
        var tenantContextMock = CreateTenantContextMock();

        var provider = new TenantConfigurationProvider(
            memoryCache, cacheMock.Object, repoMock.Object, tenantContextMock.Object);

        var result = await provider.IsRuleEnabledAsync(
            new RuleCode("NONEXISTENT_RULE"));

        result.Should().BeFalse();
    }
}
