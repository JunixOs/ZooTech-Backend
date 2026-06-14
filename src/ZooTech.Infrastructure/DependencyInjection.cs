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
using ZooTech.Infrastructure.Time;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Module_Fecundacion.Interfaces;
using ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;

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
            options.UseSqlServer(connectionString));

        // ============================================
        // Transversal
        // ============================================

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        // ============================================
        // Repositories
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
        // Transversal
        // ============================================

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ZooTech.Application.Common.Configuration.IVacunosConfiguration, ZooTech.Infrastructure.Configuration.VacunosConfiguration>();

        // ============================================
        // Repositories
        // ============================================

        services.AddScoped<ICeloRepository, CeloRepository>();
        services.AddScoped<IOrdenioRepository, OrdenioRepository>();
        services.AddScoped<IVacunoRepository, VacunoRepository>();
        services.AddScoped<IVacunoQueryRepository, VacunoRepository>();
        services.AddScoped<ITriajeRepository, TriajeRepository>();
        services.AddScoped<ITipoPesoRepository, TipoPesoRepository>();
        services.AddScoped<IFecundacionRepository, FecundacionRepository>();

        // ============================================
        // Reports
        // ============================================

        services.AddScoped<ZooTech.Application.Common.Gateway.Services.IArbolGenealogicoExportService, ZooTech.Infrastructure.Reports.Vacunos.ArbolGenealogicoExcelExportService>();

        return services;
    }
}

