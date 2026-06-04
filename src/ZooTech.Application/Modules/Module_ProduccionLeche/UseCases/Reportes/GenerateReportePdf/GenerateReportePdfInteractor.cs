using ZooTech.Application.Common.Models.Reports;
using ZooTech.Application.Common.Models.Reports.Builders;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;

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
                Title = "Reporte de Producción Diaria de Leche",
                GeneratedAt = DateTime.Now,
                Company = "ZooTech Platform",
                Department = "Producción de Leche"
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
        var fechaDesde = query.FechaDesde?.ToString("yyyyMMdd");
        var fechaHasta = query.FechaHasta?.ToString("yyyyMMdd");
        var vacunoId = query.VacunoId.HasValue ? $"_vacuno_{query.VacunoId}" : string.Empty;

        var fecha_Now = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var name = "reporte_produccion";

        if ( string.IsNullOrEmpty(fechaDesde) && string.IsNullOrEmpty(fechaHasta))
        {
            return $"{name}_{vacunoId}_{fecha_Now}.pdf";
        }

        return $"{name}_{fechaDesde}_{fechaHasta}{vacunoId}_{fecha_Now}.pdf";
    }
}


