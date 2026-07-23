using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using ZooTech.API.IntegrationTests.Seeders;
using ZooTech.API.IntegrationTests.Support;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Infrastructure.Caching;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.API.IntegrationTests;

public sealed class ZooTechApiFactory : WebApplicationFactory<Program>
{
    public const string DefaultTenantHost = "zootecniaunas.zentrycorp.local";
    private const string CatalogDatabaseName = "IntegrationTest_Catalog";
    private const string GanaderiaDatabaseName = "IntegrationTest_Ganaderia";

    private static readonly InMemoryDatabaseRoot CatalogDatabaseRoot = new();
    private static readonly InMemoryDatabaseRoot GanaderiaDatabaseRoot = new();
    private static readonly object SeedLock = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IConnectionMultiplexer>();
            services.RemoveAll<GarnetCacheConnection>();
            services.RemoveAll<IAppCacheService>();
            services.RemoveAll<IAppAuditService>();
            services.RemoveAll<ITenantConfigurationProvider>();
            services.RemoveAll<ITenantDbContextFactory>();
            services.RemoveAll<IGanaderiaDbContextFactory>();
            services.RemoveAll<DbContextOptions<TenantCatalogDb>>();
            services.RemoveAll<TenantCatalogDb>();

            services.AddSingleton<IAppCacheService, NullCacheService>();
            services.AddSingleton<IAppAuditService, NoOpAuditService>();
            services.AddScoped<ITenantConfigurationProvider, TestTenantConfigurationProvider>();
            services.AddScoped<ITenantDbContextFactory, InMemoryTenantDbContextFactory>();
            services.AddScoped<IGanaderiaDbContextFactory, InMemoryGanaderiaDbContextFactory>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    _ => { });

            services.AddSingleton(CreateCatalogOptions());
            services.AddScoped<TenantCatalogDb>();

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(_ => true)
                    .Build();
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        lock (SeedLock)
        {
            using var scope = host.Services.CreateScope();
            var catalogDb = scope.ServiceProvider.GetRequiredService<TenantCatalogDb>();
            catalogDb.SeedIntegrationTenants();
            catalogDb.SaveChanges();

            var ganaderiaFactory = scope.ServiceProvider.GetRequiredService<IGanaderiaDbContextFactory>();
            using var ganaderiaDb = ganaderiaFactory.CreateDbContextBySpecificDatabaseName(GanaderiaDatabaseName);
            ganaderiaDb.SeedBaseCatalogs();
            ganaderiaDb.SeedVacunosBasic();
            ganaderiaDb.SeedGenealogia();
            ganaderiaDb.SeedFecundacionCatalogs();
            ganaderiaDb.SeedVacunosReportes();
            ganaderiaDb.SaveChanges();
        }

        return host;
    }

    public HttpClient CreateTenantClient(string tenantHost = DefaultTenantHost)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });
        client.DefaultRequestHeaders.Add("X-Tenant-Url", tenantHost);
        return client;
    }

    private static DbContextOptions<TenantCatalogDb> CreateCatalogOptions()
        => new DbContextOptionsBuilder<TenantCatalogDb>()
            .UseInMemoryDatabase(CatalogDatabaseName, CatalogDatabaseRoot)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

    private sealed class InMemoryTenantDbContextFactory : ITenantDbContextFactory
    {
        public TenantCatalogDb CreateDbContextByTenantContext() => new(CreateCatalogOptions());

        public TenantCatalogDb CreateDbContextBySettingsValue() => new(CreateCatalogOptions());
    }

    private sealed class InMemoryGanaderiaDbContextFactory : IGanaderiaDbContextFactory
    {
        private static DbContextOptions<GanaderiaDbContext> CreateOptions()
            => new DbContextOptionsBuilder<GanaderiaDbContext>()
                .UseInMemoryDatabase(GanaderiaDatabaseName, GanaderiaDatabaseRoot)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        public GanaderiaDbContext CreateDbContextByTenantContext() => new(CreateOptions());

        public GanaderiaDbContext CreateDbContextBySpecificDatabaseName(
            string databaseName,
            bool useAdminLogin = false) => new(CreateOptions());
    }

    private sealed class NoOpAuditService : IAppAuditService
    {
        public Task AuditEventAsync(AuditEventInfo auditEventInfo) => Task.CompletedTask;

        public Task AuditErrorAsync(AuditErrorInfo auditErrorInfo) => Task.CompletedTask;
    }
}
