using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;
using ZooTech.Infrastructure.Reports.Vacunos;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Reports.Vacunos;

public sealed class TenantReportBrandingProviderTests
{
    private static readonly byte[] FirstPng =
        [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x01];
    private static readonly byte[] SecondPng =
        [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x02];

    [Fact]
    public async Task GetAsync_WhenBrandingIsValid_CachesTenantResult()
    {
        await using var db = CreateDatabase();
        await SeedTenantAsync(db, 1, "Tenant Uno", "https://cdn.test/tenant-1.png");
        var handler = new StubHttpMessageHandler(_ => ImageResponse(FirstPng));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = CreateSut(db, CreateTenantContext(1, "Tenant Uno"), handler, cache);

        var first = await sut.GetAsync();
        var second = await sut.GetAsync();

        first.DisplayName.Should().Be("Tenant Uno");
        first.LogoContent.Should().Equal(FirstPng);
        second.Should().BeSameAs(first);
        handler.RequestCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAsync_WhenLogoRequestFails_ReturnsAndCachesFallback()
    {
        await using var db = CreateDatabase();
        await SeedTenantAsync(db, 1, "Tenant Uno", "https://cdn.test/missing.png");
        var handler = new StubHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.NotFound));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = CreateSut(db, CreateTenantContext(1, "Tenant Uno"), handler, cache);

        var first = await sut.GetAsync();
        var second = await sut.GetAsync();

        first.DisplayName.Should().Be("Tenant Uno");
        first.LogoContent.Should().BeNull();
        second.Should().BeSameAs(first);
        handler.RequestCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAsync_WhenLogoIsMissing_ReturnsFallbackWithoutHttpRequest()
    {
        await using var db = CreateDatabase();
        await SeedTenantAsync(db, 1, "Tenant Uno", null);
        var handler = new StubHttpMessageHandler(_ => ImageResponse(FirstPng));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = CreateSut(db, CreateTenantContext(1, "Tenant Uno"), handler, cache);

        var result = await sut.GetAsync();

        result.DisplayName.Should().Be("Tenant Uno");
        result.LogoContent.Should().BeNull();
        handler.RequestCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAsync_WhenLogoContentTypeIsInvalid_ReturnsFallback()
    {
        await using var db = CreateDatabase();
        await SeedTenantAsync(db, 1, "Tenant Uno", "https://cdn.test/logo.png");
        var handler = new StubHttpMessageHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(FirstPng)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        });
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = CreateSut(db, CreateTenantContext(1, "Tenant Uno"), handler, cache);

        var result = await sut.GetAsync();

        result.LogoContent.Should().BeNull();
        handler.RequestCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAsync_WhenLogoDownloadThrows_ReturnsAndCachesFallback()
    {
        await using var db = CreateDatabase();
        await SeedTenantAsync(db, 1, "Tenant Uno", "https://cdn.test/tls-error.png");
        var handler = new ThrowingHttpMessageHandler();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = CreateSut(db, CreateTenantContext(1, "Tenant Uno"), handler, cache);

        var first = await sut.GetAsync();
        var second = await sut.GetAsync();

        first.LogoContent.Should().BeNull();
        second.Should().BeSameAs(first);
        handler.RequestCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAsync_UsesIndependentBrandingForEachTenant()
    {
        await using var db = CreateDatabase();
        await SeedTenantAsync(db, 1, "Tenant Uno", "https://cdn.test/tenant-1.png");
        await SeedTenantAsync(db, 2, "Tenant Dos", "https://cdn.test/tenant-2.png");
        var handler = new StubHttpMessageHandler(request =>
            ImageResponse(
                request.RequestUri!.AbsolutePath.Contains("tenant-1")
                    ? FirstPng
                    : SecondPng));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var firstProvider = CreateSut(db, CreateTenantContext(1, "Tenant Uno"), handler, cache);
        var secondProvider = CreateSut(db, CreateTenantContext(2, "Tenant Dos"), handler, cache);

        var first = await firstProvider.GetAsync();
        var second = await secondProvider.GetAsync();

        first.DisplayName.Should().Be("Tenant Uno");
        second.DisplayName.Should().Be("Tenant Dos");
        first.LogoContent.Should().Equal(FirstPng);
        second.LogoContent.Should().Equal(SecondPng);
        handler.RequestCount.Should().Be(2);
    }

    private static TenantReportBrandingProvider CreateSut(
        TenantCatalogDb db,
        ITenantContext tenantContext,
        HttpMessageHandler handler,
        IMemoryCache cache)
    {
        var tenantDbContextFactory = new Mock<ITenantDbContextFactory>();
        tenantDbContextFactory.Setup(f => f.CreateDbContextBySettingsValue()).Returns(db);

        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory
            .Setup(factory => factory.CreateClient("TenantReportBranding"))
            .Returns(new HttpClient(handler, disposeHandler: false));

        return new TenantReportBrandingProvider(
            tenantDbContextFactory.Object,
            tenantContext,
            clientFactory.Object,
            cache,
            NullLogger<TenantReportBrandingProvider>.Instance);
    }

    private static ITenantContext CreateTenantContext(int tenantId, string displayName)
    {
        var context = new Mock<ITenantContext>();
        context.SetupGet(value => value.TenantId).Returns(tenantId);
        context.SetupGet(value => value.DisplayName).Returns(displayName);
        return context.Object;
    }

    private static TenantCatalogDb CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<TenantCatalogDb>()
            .UseInMemoryDatabase($"branding-{Guid.NewGuid():N}")
            .Options;
        return new TenantCatalogDb(options);
    }

    private static async Task SeedTenantAsync(
        TenantCatalogDb db,
        int id,
        string displayName,
        string? logoUrl)
    {
        var entity = new tenant
        {
            id = id,
            code = $"tenant-{id}",
            subdomain = $"tenant-{id}",
            display_name = displayName,
            legal_name = displayName,
            email = $"tenant-{id}@test.local",
            phone = "000",
            timezone = "America/Lima",
            status = "ACTIVE"
        };

        if (logoUrl is not null)
        {
            entity.tenant_branding = new tenant_branding
            {
                id = id,
                tenant_id = id,
                logo_url = logoUrl,
                tenant = entity
            };
        }

        db.tenants.Add(entity);
        await db.SaveChangesAsync();
    }

    private static HttpResponseMessage ImageResponse(byte[] content)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(content)
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        return response;
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestCount++;
            return Task.FromResult(responseFactory(request));
        }
    }

    private sealed class ThrowingHttpMessageHandler : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestCount++;
            throw new HttpRequestException("TLS handshake failed");
        }
    }
}
