namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReporteExcel;

public record GenerateReporteExcelQuery
{
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
    public long? VacunoId { get; init; }

    public GenerateReporteExcelQuery(DateTime? fechaDesde = null, DateTime? fechaHasta = null, long? vacunoId = null)
    {
        FechaDesde = fechaDesde;
        FechaHasta = fechaHasta;
        VacunoId = vacunoId;
    }
}
