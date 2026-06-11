using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Time;
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

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        

        // ============================================
        // Configuration Options
        // ============================================
        services.Configure<ZooTech.Infrastructure.Storage.ReportStorageOptions>(options => 
        {
            options.ReportesBasePath = configuration["StorageConfig:ReportesBasePath"] ?? options.ReportesBasePath;
            options.ReportesVacunosPath = configuration["StorageConfig:ReportesVacunosPath"] ?? options.ReportesVacunosPath;
            options.ReportesUrlBase = configuration["StorageConfig:ReportesUrlBase"] ?? options.ReportesUrlBase;
        });
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

        services.AddScoped<ICeloRepository, CeloRepository>();
        services.AddScoped<IOrdenioRepository, OrdenioRepository>();
        services.AddScoped<IVacunoRepository, VacunoRepository>();
        services.AddScoped<ITriajeRepository, TriajeRepository>();
        services.AddScoped<ITipoPesoRepository, TipoPesoRepository>();

        // Reportes Repositories & Services
        services.AddScoped<ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos.IReporteVacunoReadRepository, ZooTech.Infrastructure.Persistence.Repositories.ReporteVacunoReadRepository>();
        services.AddScoped<ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte.IRegistroVacunoReadRepository, ZooTech.Infrastructure.Persistence.Repositories.RegistroVacunoReadRepository>();
        services.AddScoped<ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte.IRegistroVacunoExcelReportService, ZooTech.Infrastructure.Reports.RegistroVacunoExcelReportService>();
        services.AddScoped<ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte.IRegistroVacunoPdfReportService, ZooTech.Infrastructure.Reports.RegistroVacunoPdfReportService>();
        services.AddScoped<ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos.IListadoVacunosReportFileService, ZooTech.Infrastructure.Reports.ListadoVacunosReportFileService>();

        return services;
    }
}
