namespace ZooTech.Application.Common.Models.Reports;

/// <summary>
/// Modelo para representar un item de producción diaria
/// </summary>
public record ProduccionDiariaItemReport
{
    public DateTime Fecha { get; init; }
    public decimal TotalLitros { get; init; }
    public decimal LitrosEstandar { get; init; }
    public int CantidadOrdenios { get; init; }
    public decimal LitrosReales => TotalLitros;
    public decimal Diferencia => TotalLitros - LitrosEstandar;
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
    /// Total de litros reales en el período
    /// </summary>
    public decimal TotalLitrosRealesPeriodo => TotalLitrosPeriodo;

    /// <summary>
    /// Total de litros estándar en el período
    /// </summary>
    public decimal TotalLitrosEstandarPeriodo => Items.Sum(x => x.LitrosEstandar);

    /// <summary>
    /// Diferencia acumulada entre producción real y estándar
    /// </summary>
    public decimal DiferenciaTotalPeriodo => TotalLitrosRealesPeriodo - TotalLitrosEstandarPeriodo;

    /// <summary>
    /// Total de ordenios registrados
    /// </summary>
    public int TotalOrdenios => Items.Sum(x => x.CantidadOrdenios);

    /// <summary>
    /// Promedio de litros por ordenío
    /// </summary>
    public decimal PromedioPorOrdenio => TotalOrdenios > 0 ? TotalLitrosPeriodo / TotalOrdenios : 0;
}
