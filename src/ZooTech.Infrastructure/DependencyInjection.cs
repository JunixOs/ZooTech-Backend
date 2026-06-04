using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró ConnectionStrings:DefaultConnection.");

        services.AddDbContext<GanaderiaDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        // ============================================
        // Repositories
        // ============================================

        services.AddScoped<ICeloRepository, CeloRepository>();
        services.AddScoped<IOrdenioRepository, OrdenioRepository>();

        return services;
    }
}