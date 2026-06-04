using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Infrastructure.Persistence.Context;

using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Persistence.Repositories;
using ZooTech.Infrastructure.Reports;
using ZooTech.Infrastructure.Storage;
using ZooTech.Infrastructure.Time;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ============================================
        // Configuration Options
        // ============================================
        services.Configure<ReportStorageOptions>(options => 
        {
            options.ReportesBasePath = configuration["StorageConfig:ReportesBasePath"] ?? options.ReportesBasePath;
            options.ReportesVacunosPath = configuration["StorageConfig:ReportesVacunosPath"] ?? options.ReportesVacunosPath;
            options.ReportesUrlBase = configuration["StorageConfig:ReportesUrlBase"] ?? options.ReportesUrlBase;
        });

        // ============================================
        // Connection String
        // ============================================

        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        // ============================================
        // DbContext
        // ============================================

        int commandTimeout = 180;
        if (int.TryParse(configuration["ConnectionStrings:CommandTimeout"], out var parsedTimeout))
        {
            commandTimeout = parsedTimeout;
        }

        services.AddDbContext<GanaderiaDbContext>(options =>
        {
            // Aumentar tiempo de espera y habilitar reintentos frente a errores transitorios
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(commandTimeout); // segundos
                sqlOptions.EnableRetryOnFailure();
            });
        });

        // ============================================
        // Repositories
        // ============================================

        services.AddScoped<IReporteVacunoReadRepository, ReporteVacunoReadRepository>();
        services.AddScoped<IRegistroVacunoReadRepository, RegistroVacunoReadRepository>();
        services.AddScoped<IRegistroVacunoExcelReportService, RegistroVacunoExcelReportService>();
        services.AddScoped<IRegistroVacunoPdfReportService, RegistroVacunoPdfReportService>();
        services.AddScoped<IListadoVacunosReportFileService, ListadoVacunosReportFileService>();

        // ============================================
        // External Services
        // ============================================

        // services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        // ============================================
        // Caching
        // ============================================

        // services.AddMemoryCache();

        return services;
    }
}
