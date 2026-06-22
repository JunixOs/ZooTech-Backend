using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Features;
using ZooTech.Infrastructure.Tenant;
using ZooTech.Infrastructure.Common.Time;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionName = configuration["Database:ConnectionName"] ?? "DefaultConnection";
        var connectionString = configuration.GetConnectionString(connectionName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"No se encontro una cadena de conexion valida en ConnectionStrings:{connectionName}.");
        }

        services.AddDbContext<GanaderiaDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        // Multi-tenant context and feature configuration.
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<IFeatureService, DevFeatureService>();

        services.AddScoped<ICeloRepository, CeloRepository>();
        services.AddScoped<IOrdenioRepository, OrdenioRepository>();
        services.AddScoped<IVacunoRepository, VacunoRepository>();
        services.AddScoped<ITriajeRepository, TriajeRepository>();
        services.AddScoped<ITipoPesoRepository, TipoPesoRepository>();

        return services;
    }
}

