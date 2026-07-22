using ClosedXML.Excel;
using ScottPlot;
using System.Globalization;
using ZooTech.Application.Common.Gateway.Services;

namespace ZooTech.Infrastructure.Common.Services.ExcelGenerator;

public sealed class ExcelComparationGeneratorService : IExcelComparationGeneratorService
{
    // Misma paleta de colores del sistema
    private static readonly XLColor Primary = XLColor.FromHtml("#802b4d");
    private static readonly XLColor PrimaryLight = XLColor.FromHtml("#ecdfe4");
    private static readonly XLColor TextDark = XLColor.FromHtml("#2d2d2d");
    private static readonly XLColor BorderSoft = XLColor.FromHtml("#d9bfca");
    private static readonly XLColor PositiveGreen = XLColor.FromHtml("#16a34a");
    private static readonly XLColor NegativeRed = XLColor.FromHtml("#dc2626");

    public byte[] Generate(ExcelComparationReportRequest request)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(
            SanitizeSheetName(request.SheetName));

        var currentRow = 1;

        // ─── 1. Encabezado ──────────────────────────────────────────
        currentRow = RenderHeader(worksheet, currentRow, request);

        // ─── 2. Tabla comparativa ───────────────────────────────────
        var tableStartRow = currentRow;
        currentRow = RenderComparationTable(worksheet, currentRow, request);

        // ─── 3. Totales ─────────────────────────────────────────────
        currentRow = RenderTotals(worksheet, currentRow, request);

        // ─── 4. Gráfico de líneas (ScottPlot → PNG → incrustado) ───
        RenderChart(worksheet, tableStartRow, request);

        // Auto-ajustar columnas de la tabla (A–D)
        for (int col = 1; col <= 4; col++)
            worksheet.Column(col).AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // ─── Encabezado ─────────────────────────────────────────────────

    private static int RenderHeader(
        IXLWorksheet ws, int row, ExcelComparationReportRequest req)
    {
        ws.Cell(row, 1).Value = req.Title;
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontSize = 16;
        ws.Cell(row, 1).Style.Font.FontColor = Primary;
        row++;

        ws.Cell(row, 1).Value =
            $"Generado: {req.GeneratedAtUtc:yyyy-MM-dd HH:mm} UTC";
        ws.Cell(row, 1).Style.Font.FontColor = TextDark;
        row++;

        if (!string.IsNullOrWhiteSpace(req.FilterLine))
        {
            ws.Cell(row, 1).Value = req.FilterLine;
            ws.Cell(row, 1).Style.Font.FontColor = TextDark;
            row++;
        }

        // Información del vacuno (solo para comparativo individual)
        if (req.VacunoNombre is not null)
        {
            ws.Cell(row, 1).Value =
                $"Vacuno: {req.VacunoCodigo} — {req.VacunoNombre}";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontColor = Primary;
            row++;
        }

        row++; // Línea en blanco antes de la tabla
        return row;
    }

    // ─── Tabla comparativa ──────────────────────────────────────────

    private static int RenderComparationTable(
        IXLWorksheet ws, int startRow, ExcelComparationReportRequest req)
    {
        var headers = new[]
        {
            "Fecha",
            req.SerieRealLabel,
            req.SerieComparacionLabel,
            "Diferencia (L)"
        };

        // Encabezados de tabla
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(startRow, i + 1);
            cell.Value = headers[i];
            cell.Style.Fill.BackgroundColor = Primary;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
        }

        // Filas de datos
        var currentRow = startRow + 1;
        for (int i = 0; i < req.Rows.Count; i++)
        {
            var item = req.Rows[i];
            var diferencia = item.LitrosReales - item.LitrosComparacion;
            var isAlternate = i % 2 == 1;

            ws.Cell(currentRow, 1).Value =
                item.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            ws.Cell(currentRow, 2).Value = item.LitrosReales;
            ws.Cell(currentRow, 3).Value = item.LitrosComparacion;

            var diffCell = ws.Cell(currentRow, 4);
            diffCell.Value = diferencia;
            diffCell.Style.Font.FontColor =
                diferencia >= 0 ? PositiveGreen : NegativeRed;
            diffCell.Style.Font.Bold = true;

            // Estilo cebra + borde
            var rowRange = ws.Range(currentRow, 1, currentRow, 4);
            rowRange.Style.Font.FontColor = TextDark;
            if (isAlternate)
                rowRange.Style.Fill.BackgroundColor = PrimaryLight;
            rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.BottomBorderColor = BorderSoft;

            currentRow++;
        }

        return currentRow;
    }

    // ─── Totales ────────────────────────────────────────────────────

    private static int RenderTotals(
        IXLWorksheet ws, int row, ExcelComparationReportRequest req)
    {
        row++; // Línea en blanco

        var totalReal = req.Rows.Sum(r => r.LitrosReales);
        var totalComparacion = req.Rows.Sum(r => r.LitrosComparacion);
        var totalDiferencia = totalReal - totalComparacion;

        ws.Cell(row, 1).Value = "TOTALES";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontColor = Primary;

        ws.Cell(row, 2).Value = totalReal;
        ws.Cell(row, 2).Style.Font.Bold = true;

        ws.Cell(row, 3).Value = totalComparacion;
        ws.Cell(row, 3).Style.Font.Bold = true;

        var totalDiffCell = ws.Cell(row, 4);
        totalDiffCell.Value = totalDiferencia;
        totalDiffCell.Style.Font.Bold = true;
        totalDiffCell.Style.Font.FontColor =
            totalDiferencia >= 0 ? PositiveGreen : NegativeRed;

        var totalsRange = ws.Range(row, 1, row, 4);
        totalsRange.Style.Border.TopBorder = XLBorderStyleValues.Double;
        totalsRange.Style.Border.TopBorderColor = Primary;

        return row + 2;
    }

    // ─── Gráfico con ScottPlot ──────────────────────────────────────

    private static void RenderChart(
        IXLWorksheet ws, int tableStartRow, ExcelComparationReportRequest req)
    {
        if (req.Rows.Count < 2) return;

        // Crear el gráfico con ScottPlot 5
        var plot = new Plot();
        plot.Title(req.Title);
        plot.YLabel("Litros");
        plot.XLabel("Fecha");

        // Preparar datos
        var fechas = req.Rows
            .Select(r => r.Fecha.ToOADate())
            .ToArray();
        var litrosReales = req.Rows
            .Select(r => (double)r.LitrosReales)
            .ToArray();
        var litrosComparacion = req.Rows
            .Select(r => (double)r.LitrosComparacion)
            .ToArray();

        // Serie Real
        var scatterReal = plot.Add.Scatter(fechas, litrosReales);
        scatterReal.LegendText = req.SerieRealLabel;
        scatterReal.Color = new ScottPlot.Color(128, 43, 77);  // #802b4d
        scatterReal.LineWidth = 2;

        // Serie Comparación
        var scatterComp = plot.Add.Scatter(fechas, litrosComparacion);
        scatterComp.LegendText = req.SerieComparacionLabel;
        scatterComp.Color = new ScottPlot.Color(107, 33, 168); // #6b21a8
        scatterComp.LineWidth = 2;

        // Formato del eje X como fechas
        plot.Axes.DateTimeTicksBottom();
        plot.ShowLegend();

        // Renderizar a PNG en memoria
        var imageBytes = plot.GetImageBytes(800, 400, ImageFormat.Png);

        // Incrustar imagen en el Excel al costado de la tabla (columna F)
        using var imageStream = new MemoryStream(imageBytes);
        var picture = ws.AddPicture(imageStream)
            .MoveTo(ws.Cell(tableStartRow, 6))
            .WithSize(800, 400);
    }

    // ─── Utilidades ─────────────────────────────────────────────────

    private static string SanitizeSheetName(string name)
    {
        var invalid = new[] { ':', '\\', '/', '?', '*', '[', ']' };
        var sanitized = new string(name
            .Where(c => !invalid.Contains(c))
            .ToArray())
            .Trim();

        if (string.IsNullOrWhiteSpace(sanitized))
            sanitized = "Comparativo";

        return sanitized.Length > 31 ? sanitized[..31] : sanitized;
    }
}
