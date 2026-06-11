namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReportePdf;

/// <summary>
/// Query para generar PDF del reporte de producción diaria
/// </summary>
public record GenerateReportePdfQuery
{
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
    public long? VacunoId { get; init; }

    public GenerateReportePdfQuery(DateTime? fechaDesde = null, DateTime? fechaHasta = null, long? vacunoId = null)
    {
        FechaDesde = fechaDesde;
        FechaHasta = fechaHasta;
        VacunoId = vacunoId;
    }
}
