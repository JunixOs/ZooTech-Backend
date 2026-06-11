using ZooTech.Application.Common.Models.Reports;
using ZooTech.Application.Common.Models.Reports.Builders;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Entities.Configuration;
namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReportePdf;

/// <summary>
/// Interactor para generar el PDF del reporte de producción diaria
/// </summary>
public class GenerateReportePdfInteractor : IGenerateReportePdfInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IPdfGeneratorService _pdfGenerator;

    public GenerateReportePdfInteractor(IOrdenioRepository repository, IPdfGeneratorService pdfGenerator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pdfGenerator = pdfGenerator ?? throw new ArgumentNullException(nameof(pdfGenerator));
    }

    public async Task<GenerateReportePdfOutput> HandleAsync(GenerateReportePdfQuery query, CancellationToken cancellationToken)
    {
        // Obtener datos del reporte
        var items = await _repository.GetProduccionDiariaAsync(
            query.FechaDesde,
            query.FechaHasta,
            query.VacunoId,
            cancellationToken);

        // Crear modelo de reporte
        var reporte = new ReporteProduccionDiaria
        {
            Metadata = new ReportMetadata
            {
                Title = ConfigSettings.Reporteleche.ReportDailyProductionTitle,
                GeneratedAt = DateTime.Now,
                Company = ConfigSettings.Reporteleche.ReportCompanyName,
                Department = ConfigSettings.Reporteleche.ReportDepartmentName
            },
            VacunoId = query.VacunoId,
            FechaDesde = query.FechaDesde,
            FechaHasta = query.FechaHasta,
            Items = items
                .Select(x => new ProduccionDiariaItemReport
                {
                    Fecha = x.Fecha,
                    TotalLitros = x.TotalLitros,
                    CantidadOrdenios = x.CantidadOrdenios
                })
                .ToList()
        };

        // Generar PDF
        var builder = new ProduccionDiariaPdfBuilder(reporte);
        var pdfBytes = await _pdfGenerator.GenerateAsync(
            reporte.Metadata.Title,
            container => builder.Compose(container),
            cancellationToken);

        // Generar nombre de archivo
        var fileName = GenerarNombreArchivo(query);

        return new GenerateReportePdfOutput(pdfBytes, fileName);
    }

    private static string GenerarNombreArchivo(GenerateReportePdfQuery query)
    {
        var fechaDesde = query.FechaDesde?.ToString(ConfigSettings.Reporteleche.ReportFileDatetimeFormat);
        var fechaHasta = query.FechaHasta?.ToString(ConfigSettings.Reporteleche.ReportFileDatetimeFormat);
        var vacunoId = query.VacunoId.HasValue ? $"_{ConfigSettings.Reporteleche.ReportFileCattlePrefix}_{query.VacunoId}" : string.Empty;

        var fecha_Now = DateTime.Now.ToString(ConfigSettings.Reporteleche.ReportFileDatetimeFormat);
        var name = ConfigSettings.Reporteleche.ReportFileBaseName;

        if ( string.IsNullOrEmpty(fechaDesde) && string.IsNullOrEmpty(fechaHasta))
        {
            return $"{name}_{vacunoId}_{fecha_Now}.{ConfigSettings.Reporteleche.ReportFileExtension}";
        }

        return $"{name}_{fechaDesde}_{fechaHasta}{vacunoId}_{fecha_Now}.{ConfigSettings.Reporteleche.ReportFileExtension}";
    }
}


