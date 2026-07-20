using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using StackExchange.Redis;
using ZooTech.API.IntegrationTests.Seeders;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.API.IntegrationTests;

public sealed class ZooTechApiFactory : WebApplicationFactory<Program>
{
    public const string DefaultTenantHost = "zootecniaunas.zentrycorp.local";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:TenantCatalogConnection", "Server=(localdb)\\mssqllocaldb;Database=ZooTech_Catalog;Trusted_Connection=True;" },
                { "ConnectionStrings:AdminTenantTemplate", "Server=(localdb)\\mssqllocaldb;Database={TenantDb};Trusted_Connection=True;" }
            });
        });
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IConnectionMultiplexer>();
            services.RemoveAll<GarnetCacheConnection>();
            services.RemoveAll<IAppCacheService>();
            services.RemoveAll<IAppAuditService>();

            services.AddSingleton<IAppCacheService, NullCacheService>();
            services.AddSingleton<IAppAuditService, NoOpAuditService>();

            // Reemplazar bases de datos por InMemory
            services.RemoveAll<ITenantDbContextFactory>();
            services.RemoveAll<IGanaderiaDbContextFactory>();

            services.RemoveAll<DbContextOptions<TenantCatalogDb>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<TenantCatalogDb>();

            // Bypass Authorization for Integration Tests
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAssertion(_ => true)
                    .Build();
            });

            var catalogOptions = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseInMemoryDatabase("IntegrationTest_Catalog")
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            services.AddSingleton(catalogOptions);
            services.AddScoped<TenantCatalogDb>();

            // Seed TenantCatalogDb with settings
            // Use Mock TenantConfigurationProvider to provide default settings for testing
            services.AddScoped<ZooTech.Application.Common.Gateway.Parametrization.ITenantConfigurationProvider>(sp =>
            {
                var mockProvider = new Moq.Mock<ZooTech.Application.Common.Gateway.Parametrization.ITenantConfigurationProvider>();
                
                // Configurar valores por defecto requeridos por las pruebas
                mockProvider.Setup(p => p.GetSettingAsync(Moq.It.IsAny<ZooTech.Domain.Configuration.SettingDefinition<int>>()))
                            .ReturnsAsync(255);
                            
                // Override specific ones if needed
                mockProvider.Setup(p => p.GetSettingAsync(Moq.It.Is<ZooTech.Domain.Configuration.SettingDefinition<int>>(s => s.Code == "VACUNOS_ARBOL_MIN_NIVELES")))
                            .ReturnsAsync(1);
                mockProvider.Setup(p => p.GetSettingAsync(Moq.It.Is<ZooTech.Domain.Configuration.SettingDefinition<int>>(s => s.Code == "VACUNOS_ARBOL_MAX_NIVELES")))
                            .ReturnsAsync(4);
                            
                mockProvider.Setup(p => p.GetSettingAsync(Moq.It.IsAny<ZooTech.Domain.Configuration.SettingDefinition<bool>>()))
                            .ReturnsAsync(false);
                            
                return mockProvider.Object;
            });

            services.AddScoped<ITenantDbContextFactory, InMemoryTenantDbContextFactory>();
            services.AddScoped<IGanaderiaDbContextFactory, InMemoryGanaderiaDbContextFactory>();

        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var catalogDb = scope.ServiceProvider.GetRequiredService<TenantCatalogDb>();
        
        if (!catalogDb.tenants.Any())
        {
            var tenantsToSeed = new[] { "zootecniaunas", "elroble", "lacteosdelvalle", "losandes", "tenant-int" };
            foreach(var subdomain in tenantsToSeed)
            {
                var newTenant = new tenant
                {
                    subdomain = subdomain,
                    code = subdomain.Substring(0, Math.Min(4, subdomain.Length)).ToUpper(),
                    status = "ACTIVE",
                    email = $"admin@{subdomain}.zentrycorp.local",
                    display_name = subdomain,
                    legal_name = $"{subdomain} SAC",
                    phone = "123456",
                    timezone = "UTC"
                };
                catalogDb.tenants.Add(newTenant);
                
                catalogDb.tenant_database_connections.Add(new tenant_database_connection
                {
                    tenant = newTenant,
                    database_name = $"ZooTech_{subdomain}_Db",
                    is_active = true
                });
            }
            catalogDb.SaveChanges();
        }

        // Sembrar GanaderiaDbContext (Tenant Db)
        var ganaderiaFactory = scope.ServiceProvider.GetRequiredService<IGanaderiaDbContextFactory>();
        using var ganaderiaDb = ganaderiaFactory.CreateDbContextBySpecificDatabaseName("IntegrationTest_Ganaderia");
        
        lock (_seedLock)
        {
            // --- SEEDERS ---
            ganaderiaDb.SeedBaseCatalogs();
            ganaderiaDb.SeedVacunosBasic();
            ganaderiaDb.SeedGenealogia();



            ganaderiaDb.SaveChanges();
        }

        return host;
    }

    private static readonly object _seedLock = new object();

    public HttpClient CreateTenantClient(string tenantHost = DefaultTenantHost)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });
        client.DefaultRequestHeaders.Add("X-Tenant-Url", tenantHost);
        return client;
    }

    private sealed class NoOpAuditService : IAppAuditService
    {
        public Task AuditEventAsync(AuditEventInfo auditEventInfo) => Task.CompletedTask;
        public Task AuditErrorAsync(AuditErrorInfo auditErrorInfo) => Task.CompletedTask;
    }

    private sealed class InMemoryTenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly DbContextOptions<TenantCatalogDb> _options;
        public InMemoryTenantDbContextFactory()
        {
            _options = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseInMemoryDatabase("IntegrationTest_Catalog")
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }
        public TenantCatalogDb CreateDbContextByTenantContext() => new TenantCatalogDb(_options);
        public TenantCatalogDb CreateDbContextBySettingsValue() => new TenantCatalogDb(_options);
    }

    private sealed class InMemoryGanaderiaDbContextFactory : IGanaderiaDbContextFactory
    {
        private readonly DbContextOptions<GanaderiaDbContext> _options;
        public InMemoryGanaderiaDbContextFactory()
        {
            _options = new DbContextOptionsBuilder<GanaderiaDbContext>()
                .UseInMemoryDatabase("IntegrationTest_Ganaderia")
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }
        public GanaderiaDbContext CreateDbContextByTenantContext() => new GanaderiaDbContext(_options);
        public GanaderiaDbContext CreateDbContextBySpecificDatabaseName(string databaseName, bool useAdminLogin = false) => new GanaderiaDbContext(_options);
    }
}
