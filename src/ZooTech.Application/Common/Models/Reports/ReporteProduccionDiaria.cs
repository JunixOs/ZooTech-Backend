namespace ZooTech.Application.Common.Models.Reports;

/// <summary>
/// Modelo para representar un item de producción diaria
/// </summary>
public record ProduccionDiariaItemReport
{
    public DateTime Fecha { get; init; }
    public decimal TotalLitros { get; init; }
    public int CantidadOrdenios { get; init; }
}

/// <summary>
/// Modelo para el reporte de producción diaria de leche
/// </summary>
public class ReporteProduccionDiaria : ReportBase
{
    public long? VacunoId { get; set; }
    public string? NombreVacuno { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public List<ProduccionDiariaItemReport> Items { get; set; } = new();

    /// <summary>
    /// Total de litros en el período
    /// </summary>
    public decimal TotalLitrosPeriodo => Items.Sum(x => x.TotalLitros);

    /// <summary>
    /// Total de ordenios registrados
    /// </summary>
    public int TotalOrdenios => Items.Sum(x => x.CantidadOrdenios);

    /// <summary>
    /// Promedio de litros por ordenío
    /// </summary>
    public decimal PromedioPorOrdenio => TotalOrdenios > 0 ? TotalLitrosPeriodo / TotalOrdenios : 0;
}
