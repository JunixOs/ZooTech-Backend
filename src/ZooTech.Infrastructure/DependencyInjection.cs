using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Parametrization.Features;
using ZooTech.Application.Common.Gateway.Parametrization.Rules;
using ZooTech.Application.Common.Gateway.Parametrization.Settings;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Infrastructure.Auditing.MongoDb;
using ZooTech.Infrastructure.Caching;
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

        // Parametrization / Settings / Features / Rules
        services.AddScoped<ISettingsProvider, SettingsProvider>();
        services.AddScoped<IFeatureProvider, FeatureProvider>();
        services.AddScoped<IRuleProvider, RuleProvider>();

        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IRuleRepository, RuleRepository>();

        return services;
    }
}
