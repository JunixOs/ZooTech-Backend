using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Common.Time;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;
using ZooTech.Infrastructure.Reports;
using ZooTech.Infrastructure.Repositories;

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

        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IAnimalReportRepository, AnimalReportRepository>();
        services.AddScoped<IAnimalReportExcelService, AnimalReportExcelService>();
        services.AddScoped<IAnimalReportPdfService, AnimalReportPdfService>();
        services.AddScoped<ICeloRepository, CeloRepository>();
        services.AddScoped<IFecundacionEstadoRepository, FecundacionEstadoRepository>();
        services.AddScoped<IOrdenioRepository, OrdenioRepository>();
        services.AddScoped<IVacunoRepository, VacunoRepository>();
        services.AddScoped<ITriajeRepository, TriajeRepository>();
        services.AddScoped<ITipoPesoRepository, TipoPesoRepository>();

        return services;
    }
}
