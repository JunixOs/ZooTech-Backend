using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Parametrization.Features;
using ZooTech.Application.Common.Gateway.Parametrization.Rules;
using ZooTech.Application.Common.Gateway.Parametrization.Settings;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Infrastructure.Auditing.MongoDb;
using ZooTech.Infrastructure.Caching;
using ZooTech.Infrastructure.Parametrization;
using ZooTech.Infrastructure.Parametrization.Features;
using ZooTech.Infrastructure.Parametrization.Rules;
using ZooTech.Infrastructure.Parametrization.Settings;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var garnetConnectionString = configuration["Garnet:ConnectionString"]
            ?? throw new InvalidOperationException("Garnet:ConnectionString no configurado");
        var multiplexer = ConnectionMultiplexer.Connect(garnetConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        services.AddSingleton<GarnetCacheConnection>();
        services.AddSingleton<IAppCacheService, GarnetCacheService>();

        services.AddScoped<ITenantStore, TenantStore>();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
        services.AddScoped<ITenantDatabaseMigrator, TenantDatabaseMigrator>();
        services.AddScoped<IGanaderiaDbContextFactory, GanaderiaDbContextFactory>();
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();

        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IAppAuditService, MongoDbAudit>();

        services.AddScoped<ITenantRepository, TenantRepository>();

        services.AddScoped<GanaderiaDbContext>(sp =>
        {
            var factory = sp.GetRequiredService<ITenantDbContextFactory>();
            return factory.CreateDbContext();
        });

        // Unified Tenant Configuration (Phase 1-2)
        services.AddScoped<ITenantConfigurationRepository, TenantConfigurationRepository>();
        services.AddScoped<ITenantConfigurationProvider, TenantConfigurationProvider>();

        // Parametrization / Settings / Features / Rules (deprecated — use ITenantConfigurationProvider)
        services.AddScoped<ISettingsProvider, SettingsProvider>();
        services.AddScoped<IFeatureProvider, FeatureProvider>();
        services.AddScoped<IRuleProvider, RuleProvider>();

        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IRuleRepository, RuleRepository>();

        // Multi-Instance Sync (Phase 4)
        services.AddHostedService<ConfigInvalidationSubscriber>();

        return services;
    }
}
