using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Common.Time;
using ZooTech.Infrastructure.Features;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        services.Configure<ZooTech.Infrastructure.Storage.ReportStorageOptions>(options =>
        {
            options.ReportesBasePath = configuration["StorageConfig:ReportesBasePath"] ?? options.ReportesBasePath;
            options.ReportesVacunosPath = configuration["StorageConfig:ReportesVacunosPath"] ?? options.ReportesVacunosPath;
            options.ReportesUrlBase = configuration["StorageConfig:ReportesUrlBase"] ?? options.ReportesUrlBase;
        });

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
        services.AddScoped<ITimeProvider, ZooTech.Infrastructure.Time.SystemTimeProvider>();

        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<IFeatureService, DevFeatureService>();

        services.AddScoped<ICeloRepository, CeloRepository>();
        services.AddScoped<IFecundacionRepository, FecundacionRepository>();
        services.AddScoped<IOrdenioRepository, OrdenioRepository>();
        services.AddScoped<IVacunoRepository, VacunoRepository>();
        services.AddScoped<IVacunoListadoReadRepository, VacunoListadoReadRepository>();
        services.AddScoped<IVacunoActivityStatsReadRepository, VacunoActivityStatsReadRepository>();
        services.AddScoped<IListadoVacunosReporteReadRepository, ListadoVacunosReporteReadRepository>();
        services.AddScoped<ITriajeRepository, TriajeRepository>();
        services.AddScoped<ITipoPesoRepository, TipoPesoRepository>();
        services.AddScoped<IRegistroVacunoReadRepository, RegistroVacunoReadRepository>();
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.IRegistroVacunoExcelReportService, ZooTech.Infrastructure.Reports.RegistroVacunoExcelReportService>();
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.IRegistroVacunoPdfReportService, ZooTech.Infrastructure.Reports.RegistroVacunoPdfReportService>();

        return services;
    }
}
