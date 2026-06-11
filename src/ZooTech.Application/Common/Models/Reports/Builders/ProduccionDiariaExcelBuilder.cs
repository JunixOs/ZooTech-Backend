using ClosedXML.Excel;
using ZooTech.Domain.Entities.Configuration;

namespace ZooTech.Application.Common.Models.Reports.Builders;

public class ProduccionDiariaExcelBuilder
{
    private readonly ReporteProduccionDiaria _reporte;

    public ProduccionDiariaExcelBuilder(ReporteProduccionDiaria reporte)
    {
        _reporte = reporte ?? throw new ArgumentNullException(nameof(reporte));
    }

    public byte[] Build()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(ConfigSettings.Reporteleche.ReportDepartmentName);

        worksheet.Cell(1, 1).Value = _reporte.Metadata.Title;
        worksheet.Cell(2, 1).Value = $"Empresa: {_reporte.Metadata.Company}";
        worksheet.Cell(3, 1).Value = $"Departamento: {_reporte.Metadata.Department}";
        worksheet.Cell(4, 1).Value = $"Generado: {_reporte.Metadata.GeneratedAt:dd/MM/yyyy HH:mm:ss}";
        worksheet.Cell(5, 1).Value = $"Periodo: {FormatearFecha(_reporte.FechaDesde)} al {FormatearFecha(_reporte.FechaHasta)}";

        if (_reporte.VacunoId.HasValue)
        {
            worksheet.Cell(6, 1).Value = $"Vacuno ID: {_reporte.VacunoId.Value}";
        }

        const int headerRow = 8;
        worksheet.Cell(headerRow, 1).Value = "Fecha";
        worksheet.Cell(headerRow, 2).Value = "Produccion Real";
        worksheet.Cell(headerRow, 3).Value = "Produccion Estandar";
        worksheet.Cell(headerRow, 4).Value = "Diferencia";
        worksheet.Cell(headerRow, 5).Value = "Ordenios";

        var headerRange = worksheet.Range(headerRow, 1, headerRow, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightPink;

        var row = headerRow + 1;
        foreach (var item in _reporte.Items)
        {
            worksheet.Cell(row, 1).Value = item.Fecha;
            worksheet.Cell(row, 1).Style.DateFormat.Format = ConfigSettings.Reporteleche.ReportFileDateFormat;
            worksheet.Cell(row, 2).Value = item.LitrosReales;
            worksheet.Cell(row, 3).Value = item.LitrosEstandar;
            worksheet.Cell(row, 4).Value = item.Diferencia;
            worksheet.Cell(row, 5).Value = item.CantidadOrdenios;
            row++;
        }

        worksheet.Cell(row, 1).Value = "TOTAL";
        worksheet.Cell(row, 2).Value = _reporte.TotalLitrosRealesPeriodo;
        worksheet.Cell(row, 3).Value = _reporte.TotalLitrosEstandarPeriodo;
        worksheet.Cell(row, 4).Value = _reporte.DiferenciaTotalPeriodo;
        worksheet.Cell(row, 5).Value = _reporte.TotalOrdenios;

        var totalRange = worksheet.Range(row, 1, row, 5);
        totalRange.Style.Font.Bold = true;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string FormatearFecha(DateTime? fecha)
        => fecha.HasValue ? fecha.Value.ToString(ConfigSettings.Reporteleche.ReportFileDateFormat) : "N/A";
}
