namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReportePdf;

/// <summary>
/// Puerto de entrada para generar reporte PDF
/// </summary>
public interface IGenerateReportePdfInputPort
{
    /// <summary>
    /// Genera un PDF con el reporte de producción diaria
    /// </summary>
    /// <param name="query">Query con los filtros del reporte</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Output con los bytes del PDF</returns>
    Task<GenerateReportePdfOutput> HandleAsync(GenerateReportePdfQuery query, CancellationToken cancellationToken);
}
