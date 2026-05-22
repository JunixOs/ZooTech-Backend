using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Infrastructure.Features;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;
using ZooTech.Infrastructure.Time;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ============================================
        // Connection String
        // ============================================

        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        // ============================================
        // DbContext
        // ============================================

        services.AddDbContext<GanaderiaDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        // ============================================
        // Multi-Tenant
        // ============================================

        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, TenantContext>();

        // ============================================
        // Feature Flags
        // ============================================

        services.AddScoped<IFeatureService, DevFeatureService>();

        // ============================================
        // Repositories
        // ============================================

        services.AddScoped<ZooTech.Application.Common.Gateway.Repositories.IVacunoRepository, ZooTech.Infrastructure.Persistence.Repositories.VacunoRepository>();

        // ============================================
        // External Services
        // ============================================

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // ============================================
        // Caching
        // ============================================

        // services.AddMemoryCache();

        return services;
    }
}