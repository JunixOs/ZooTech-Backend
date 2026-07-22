namespace ZooTech.Application.Common.Gateway.Services;

/// <summary>
/// Contrato para generar reportes Excel comparativos con tabla de datos
/// y gráfico de líneas incrustado (Real vs. Estándar/Predicción).
/// </summary>
public interface IExcelComparationGeneratorService
{
    byte[] Generate(ExcelComparationReportRequest request);
}

/// <summary>
/// Request para generar un reporte comparativo en Excel.
/// </summary>
public sealed record ExcelComparationReportRequest(
    string Title,
    string SheetName,
    IReadOnlyList<ExcelComparationRow> Rows,
    DateTime GeneratedAtUtc,
    string? FilterLine = null,
    string SerieRealLabel = "Producción Real (L)",
    string SerieComparacionLabel = "Producción Estándar (L)",
    string? VacunoNombre = null,
    string? VacunoCodigo = null);

/// <summary>
/// Fila de datos para el reporte comparativo.
/// </summary>
public sealed record ExcelComparationRow(
    DateTime Fecha,
    decimal LitrosReales,
    decimal LitrosComparacion);
