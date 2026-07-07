using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Export;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Domain.Common.Interfaces;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Common.Export;
using ZooTech.Infrastructure.Common.Time;
using ZooTech.Infrastructure.Common.Services.PdfGenerator;
using ZooTech.Infrastructure.Common.Services.ExcelGenerator;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Domain.Module_Fecundacion.Interfaces;
using ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Infrastructure.Persistence.Repositories;
using ZooTech.Infrastructure.Repositories;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Infrastructure.Reports;
using ZooTech.Infrastructure.Reports.Vacunos;
using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Infrastructure.Reports;
using ZooTech.Infrastructure.Reports.Vacunos;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró ConnectionStrings:DefaultConnection.");

        services.AddDbContext<GanaderiaDbContext>(options =>
            options.UseSqlServer(connectionString));

        // ============================================
        // Transversal
        // ============================================

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ZooTech.Application.Common.Configuration.IVacunosConfiguration, ZooTech.Infrastructure.Configuration.VacunosConfiguration>();
        services.AddScoped<IExcelDocumentGenerator, ExcelDocumentGenerator>();
        services.AddScoped<IPdfDocumentGenerator, PdfDocumentGenerator>();
        services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
        services.AddScoped<IOrdeniosComparationPdfGeneratorService, PdfGenerateComparationService>();

        services.AddScoped<IExcelGeneratorService, ExcelGeneratorService>();
        // ============================================
        // Repositories
        // ============================================

        services.AddScoped<ICeloRepository, CeloRepository>();
        services.AddScoped<IOrdenioRepository, OrdenioRepository>();
        services.AddScoped<IVacunoRepository, VacunoRepository>();
        services.AddScoped<ITriajeRepository, TriajeRepository>();
        services.AddScoped<ITipoPesoRepository, TipoPesoRepository>();
        services.AddScoped<IEstadoRegistroRepository, EstadoRegistroRepository>();

        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IAnimalReportRepository, AnimalReportRepository>();
        services.AddScoped<IAnimalReportExcelService, AnimalReportExcelService>();
        services.AddScoped<IAnimalReportPdfService, AnimalReportPdfService>();

        // Repositorios Fecundación
        services.AddScoped<IFecundacionRepository, FecundacionRepository>();
        services.AddScoped<IFecundacionEstadoRepository, FecundacionEstadoRepository>();

        // Repositorios Vacuno (Read/Write Models y UoW)
        services.AddScoped<IVacunoListadoReadRepository, VacunoListadoReadRepository>();
        services.AddScoped<IVacunoActivityStatsReadRepository, VacunoActivityStatsReadRepository>();
        services.AddScoped<IVacunoGranjaReadRepository, VacunoGranjaReadRepository>();
        services.AddScoped<IVacunoMutationUnitOfWork, VacunoMutationUnitOfWork>();
        services.AddScoped<IVacunoReferenceReadRepository, VacunoReferenceReadRepository>();
        services.AddScoped<IVacunoResponseReadRepository, VacunoResponseReadRepository>();
        services.AddScoped<IListadoVacunosReporteReadRepository, ListadoVacunosReporteReadRepository>();
        services.AddScoped<IRegistroVacunoReadRepository, RegistroVacunoReadRepository>();

        // Servicios de Exportación y Reportes
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.IRegistroVacunoExcelReportService, RegistroVacunoExcelReportService>();
        services.AddScoped<ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.IRegistroVacunoPdfReportService, RegistroVacunoPdfReportService>();
        services.AddScoped<ZooTech.Application.Common.Gateway.Services.IArbolGenealogicoExportService, ZooTech.Infrastructure.Reports.Vacunos.ArbolGenealogicoExcelExportService>();

        return services;
    }
}
