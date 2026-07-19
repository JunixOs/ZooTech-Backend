using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
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
            var hoyDateOnly = DateOnly.FromDateTime(DateTime.UtcNow);

            if (!ganaderiaDb.vacunos.Any(v => v.codigo == "V001"))
            {
                var hembra = new ZooTech.Infrastructure.Persistence.Entities.vacuno
                {
                    codigo = "V001",
                    nombre = "Vaca de Prueba",
                    sexo_code = "H",
                    fecha_nacimiento = new DateOnly(2020, 1, 1),
                    tipo_adquisicion_code = "COMPRA",
                    raza_code = "HOLSTEIN",
                    color_code = "BLANCO",
                    granja_id = 1,
                    fecha_registro = hoyDateOnly,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };
                ganaderiaDb.vacunos.Add(hembra);
            }

            if (!ganaderiaDb.vacunos.Any(v => v.codigo == "M001"))
            {
                var macho = new ZooTech.Infrastructure.Persistence.Entities.vacuno
                {
                    codigo = "M001",
                    nombre = "Toro de Prueba",
                    sexo_code = "M",
                    fecha_nacimiento = new DateOnly(2020, 1, 1),
                    tipo_adquisicion_code = "COMPRA",
                    raza_code = "HOLSTEIN",
                    color_code = "BLANCO",
                    granja_id = 1,
                    fecha_registro = hoyDateOnly,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };
                ganaderiaDb.vacunos.Add(macho);
            }

            if (!ganaderiaDb.cat_tipo_fecundacions.Any(t => t.code == "MN"))
            {
                var tipoFecundacion = new ZooTech.Infrastructure.Persistence.Entities.cat_tipo_fecundacion
                {
                    nombre = "Monta Natural",
                    code = "MN"
                };
                ganaderiaDb.cat_tipo_fecundacions.Add(tipoFecundacion);
            }

            if (!ganaderiaDb.cat_resultado_fecundacions.Any(r => r.code == "POSITIVO"))
            {
                var resultado = new ZooTech.Infrastructure.Persistence.Entities.cat_resultado_fecundacion
                {
                    nombre = "Positivo",
                    code = "POSITIVO"
                };
                ganaderiaDb.cat_resultado_fecundacions.Add(resultado);
            }

            if (!ganaderiaDb.cat_razas.Any(r => r.code == "HOLSTEIN"))
            {
                ganaderiaDb.cat_razas.Add(new ZooTech.Infrastructure.Persistence.Entities.cat_raza { code = "HOLSTEIN", nombre = "Holstein" });
            }
            if (!ganaderiaDb.cat_sexos.Any(s => s.code == "H"))
            {
                ganaderiaDb.cat_sexos.Add(new ZooTech.Infrastructure.Persistence.Entities.cat_sexo { code = "H", nombre = "Hembra" });
                ganaderiaDb.cat_sexos.Add(new ZooTech.Infrastructure.Persistence.Entities.cat_sexo { code = "M", nombre = "Macho" });
            }
            if (!ganaderiaDb.cat_colors.Any(c => c.code == "BLANCO"))
            {
                ganaderiaDb.cat_colors.Add(new ZooTech.Infrastructure.Persistence.Entities.cat_color { code = "BLANCO", nombre = "Blanco" });
            }
            if (!ganaderiaDb.cat_tipo_adquisicions.Any(ta => ta.code == "COMPRA"))
            {
                ganaderiaDb.cat_tipo_adquisicions.Add(new ZooTech.Infrastructure.Persistence.Entities.cat_tipo_adquisicion { code = "COMPRA", nombre = "Compra" });
            }

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
