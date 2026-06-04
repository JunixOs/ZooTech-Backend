using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Infrastructure.Caching;
using ZooTech.Infrastructure.Persistence.Context;

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
        // Repositories
        // ============================================

        // services.AddScoped<IAnimalRepository, AnimalRepository>();

        // ============================================
        // External Services
        // ============================================

        // services.AddScoped<IJwtService, JwtService>();
        // services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        // ============================================
        // Caching
        // ============================================

        services.AddSingleton<GarnetCacheConnection>();
        services.AddSingleton<IAppCacheService, GarnetCacheService>();

        return services;
    }
}