using System.Globalization;
using ClosedXML.Excel;
using SkiaSharp;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

namespace ZooTech.Infrastructure.Common.Services.ExcelGenerator;

public sealed class ExcelGenerateComparationService : IOrdeniosComparationExcelGeneratorService
{
    internal sealed record ChartPoint(DateTime Timestamp, string Label, float Value, string? Vacuno = null);
    internal sealed record TrendPoint(DateTime Timestamp, string Label, float Value, float MovingAverage);

    private static readonly XLColor Primary = XLColor.FromHtml("#802b4d");
    private static readonly XLColor PrimaryLight = XLColor.FromHtml("#ecdfe4");
    private static readonly XLColor TextDark = XLColor.FromHtml("#2d2d2d");
    private static readonly XLColor BorderSoft = XLColor.FromHtml("#d9bfca");
    private static readonly SKColor PrimaryChart = new(128, 43, 77);
    private static readonly SKColor SecondaryChart = new(105, 35, 185);

    public byte[] GenerateOrdeniosReport(GenerateOrdeniosExcelDocument document)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Comparativo");

        RenderHeader(worksheet, GetReportTitle(document), document.GeneratedAtUtc, BuildFilters(document));

        if (document.VacunoId.HasValue)
        {
            RenderVacunoHistory(worksheet, document.Items);
        }
        else
        {
            RenderVacunosComparison(worksheet, document.Items);
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void RenderVacunoHistory(IXLWorksheet worksheet, IReadOnlyList<OrdenioListOutput> items)
    {
        var series = ComputeTrendSeries(items);

        RenderChart(
            worksheet,
            title: "Litros por ordenio vs tendencia",
            primaryLabel: "Litros",
            secondaryLabel: "Tendencia",
            primarySeries: series.Select(x => new ChartPoint(x.Timestamp, x.Label, x.Value)).ToList(),
            secondarySeries: series.Select(x => new ChartPoint(x.Timestamp, x.Label, x.MovingAverage)).ToList());

        worksheet.Cell(5, 1).Value = "Historial de produccion";
        worksheet.Cell(5, 1).Style.Font.Bold = true;
        worksheet.Cell(5, 1).Style.Font.FontColor = Primary;

        RenderTable(
            worksheet,
            startRow: 7,
            headers: new[] { "Fecha", "Litros", "Tendencia" },
            items: series,
            cellSelector: item => new object[]
            {
                item.Label,
                item.Value,
                item.MovingAverage
            });

        if (series.Count == 0)
        {
            RenderEmptyMessage(worksheet, 7, 1, "No hay datos de ordenios para comparar.");
        }
    }

    private static void RenderVacunosComparison(IXLWorksheet worksheet, IReadOnlyList<OrdenioListOutput> items)
    {
        var actual = ComputeActualSeries(items);
        var reference = ComputeReferenceSeries(items);

        RenderChart(
            worksheet,
            title: "Produccion individual vs promedio diario",
            primaryLabel: "Produccion individual",
            secondaryLabel: "Promedio diario",
            primarySeries: actual,
            secondarySeries: reference);

        worksheet.Cell(5, 1).Value = "Produccion individual";
        worksheet.Cell(5, 1).Style.Font.Bold = true;
        worksheet.Cell(5, 1).Style.Font.FontColor = Primary;

        RenderTable(
            worksheet,
            startRow: 7,
            headers: new[] { "Fecha", "Vacuno", "Litros" },
            items: actual,
            cellSelector: item => new object[]
            {
                item.Label,
                item.Vacuno ?? string.Empty,
                item.Value
            });

        if (actual.Count == 0)
        {
            RenderEmptyMessage(worksheet, 7, 1, "No hay vacunos para comparar.");
        }

        var referenceStartColumn = 5;
        worksheet.Cell(5, referenceStartColumn).Value = "Promedio diario";
        worksheet.Cell(5, referenceStartColumn).Style.Font.Bold = true;
        worksheet.Cell(5, referenceStartColumn).Style.Font.FontColor = Primary;

        RenderTable(
            worksheet,
            startRow: 7,
            startColumn: referenceStartColumn,
            headers: new[] { "Fecha", "Promedio litros" },
            items: reference,
            cellSelector: item => new object[]
            {
                item.Label,
                item.Value
            });
    }

    internal static IReadOnlyList<ChartPoint> ComputeActualSeries(IReadOnlyList<OrdenioListOutput> items)
    {
        return items
            .OrderBy(x => x.FechaHora)
            .Select(x => new ChartPoint(
                x.FechaHora,
                x.FechaHora.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                (float)x.Litros,
                x.NombreVacuno))
            .ToList();
    }

    internal static IReadOnlyList<ChartPoint> ComputeReferenceSeries(IReadOnlyList<OrdenioListOutput> items)
    {
        return items
            .GroupBy(x => x.FechaHora.Date)
            .OrderBy(g => g.Key)
            .Select(g => new ChartPoint(
                g.Key,
                g.Key.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                (float)g.Average(x => x.Litros)))
            .ToList();
    }

    internal static IReadOnlyList<TrendPoint> ComputeTrendSeries(IReadOnlyList<OrdenioListOutput> items)
    {
        var ordered = items
            .OrderBy(x => x.FechaHora)
            .ToList();

        var averages = CalculateMovingAverage(ordered.Select(x => (float)x.Litros).ToList(), 4);

        return ordered
            .Select((x, index) => new TrendPoint(
                x.FechaHora,
                x.FechaHora.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                (float)x.Litros,
                averages[index]))
            .ToList();
    }

    private static IReadOnlyList<float> CalculateMovingAverage(IReadOnlyList<float> values, int window)
    {
        var result = new List<float>();

        for (var i = 0; i < values.Count; i++)
        {
            var start = Math.Max(0, i - window + 1);
            result.Add(values.Skip(start).Take(i - start + 1).Average());
        }

        return result;
    }

    private static string GetReportTitle(GenerateOrdeniosExcelDocument document)
        => document.VacunoId.HasValue
            ? $"Comparativo de produccion de {GetVacunoDisplayName(document)}"
            : "Comparativo de produccion por vacunos";

    private static string GetVacunoDisplayName(GenerateOrdeniosExcelDocument document)
    {
        var nombreVacuno = document.Items
            .Select(x => x.NombreVacuno)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        return string.IsNullOrWhiteSpace(nombreVacuno)
            ? $"vacuno {document.VacunoId}"
            : nombreVacuno;
    }

    private static string BuildFilters(GenerateOrdeniosExcelDocument document)
    {
        return BuildFilterLine(
            ("VacunoId", document.VacunoId?.ToString()),
            ("Estado", document.EstadoOrdenioCode),
            ("Desde", document.FechaDesde?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)),
            ("Hasta", document.FechaHasta?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)));
    }

    private static void RenderHeader(IXLWorksheet worksheet, string title, DateTime generatedAtUtc, string filterLine)
    {
        worksheet.Cell(1, 1).Value = title;
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Cell(1, 1).Style.Font.FontColor = Primary;

        worksheet.Cell(2, 1).Value = $"Generado: {generatedAtUtc.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)} UTC";
        worksheet.Cell(3, 1).Value = filterLine;
    }

    private static void RenderTable<T>(
        IXLWorksheet worksheet,
        int startRow,
        string[] headers,
        IReadOnlyList<T> items,
        Func<T, object[]> cellSelector)
        => RenderTable(worksheet, startRow, 1, headers, items, cellSelector);

    private static void RenderTable<T>(
        IXLWorksheet worksheet,
        int startRow,
        int startColumn,
        string[] headers,
        IReadOnlyList<T> items,
        Func<T, object[]> cellSelector)
    {
        var currentRow = startRow;

        for (var i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(currentRow, startColumn + i);
            cell.Value = headers[i];
            cell.Style.Fill.BackgroundColor = Primary;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        currentRow++;
        for (var i = 0; i < items.Count; i++)
        {
            var values = cellSelector(items[i]);
            var isAlternate = i % 2 == 1;

            for (var col = 0; col < values.Length; col++)
            {
                SetCellValue(worksheet.Cell(currentRow, startColumn + col), values[col]);
            }

            var rowRange = worksheet.Range(currentRow, startColumn, currentRow, startColumn + values.Length - 1);
            rowRange.Style.Font.FontColor = TextDark;

            if (isAlternate)
            {
                rowRange.Style.Fill.BackgroundColor = PrimaryLight;
            }

            rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.BottomBorderColor = BorderSoft;

            currentRow++;
        }
    }

    private static void RenderEmptyMessage(IXLWorksheet worksheet, int row, int column, string message)
    {
        worksheet.Cell(row + 1, column).Value = message;
        worksheet.Cell(row + 1, column).Style.Font.FontColor = TextDark;
    }

    private static void SetCellValue(IXLCell cell, object? value)
    {
        cell.Value = value switch
        {
            string s => s,
            decimal d => d,
            float f => f,
            double d => d,
            int i => i,
            long l => l,
            DateTime dt => dt,
            bool b => b,
            null => string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }

    private static void RenderChart(
        IXLWorksheet worksheet,
        string title,
        string primaryLabel,
        string secondaryLabel,
        IReadOnlyList<ChartPoint> primarySeries,
        IReadOnlyList<ChartPoint> secondarySeries)
    {
        var chartBytes = BuildLineChartPng(title, primaryLabel, secondaryLabel, primarySeries, secondarySeries, 900, 360);
        var stream = new MemoryStream(chartBytes);

        worksheet.AddPicture(stream, "ordenios-comparativo-chart")
            .MoveTo(worksheet.Cell(5, 8));
    }

    internal static byte[] BuildLineChartPng(
        string title,
        string primaryLabel,
        string secondaryLabel,
        IReadOnlyList<ChartPoint> primarySeries,
        IReadOnlyList<ChartPoint> secondarySeries,
        int width,
        int height)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        DrawLineChart(canvas, width, height, title, primaryLabel, secondaryLabel, primarySeries, secondarySeries);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static void DrawLineChart(
        SKCanvas canvas,
        int width,
        int height,
        string title,
        string primaryLabel,
        string secondaryLabel,
        IReadOnlyList<ChartPoint> primarySeries,
        IReadOnlyList<ChartPoint> secondarySeries)
    {
        canvas.Clear(SKColors.White);

        using var titlePaint = new SKPaint
        {
            Color = new SKColor(35, 35, 35),
            TextSize = 18,
            FakeBoldText = true,
            IsAntialias = true
        };

        using var textPaint = new SKPaint
        {
            Color = new SKColor(70, 70, 70),
            TextSize = 11,
            IsAntialias = true
        };

        canvas.DrawText(title, 24, 28, titlePaint);

        var timeline = primarySeries.Concat(secondarySeries).ToList();
        if (timeline.Count == 0)
        {
            canvas.DrawText("No hay datos para mostrar.", 24, height / 2, textPaint);
            return;
        }

        var marginLeft = 56;
        var marginRight = 28;
        var marginTop = 56;
        var marginBottom = 64;
        var chartWidth = width - marginLeft - marginRight;
        var chartHeight = height - marginTop - marginBottom;
        var minTimestamp = timeline.Min(x => x.Timestamp);
        var maxTimestamp = timeline.Max(x => x.Timestamp);
        var maxValue = Math.Max(10, (float)Math.Ceiling(timeline.Max(x => x.Value) / 5f) * 5);

        float GetX(DateTime timestamp)
        {
            if (minTimestamp == maxTimestamp)
            {
                return marginLeft + chartWidth / 2f;
            }

            var ratio = (timestamp - minTimestamp).TotalMilliseconds / (maxTimestamp - minTimestamp).TotalMilliseconds;
            return marginLeft + (float)ratio * chartWidth;
        }

        float GetY(float value) => marginTop + chartHeight - value / maxValue * chartHeight;

        using var gridPaint = new SKPaint
        {
            Color = new SKColor(220, 220, 220),
            StrokeWidth = 1,
            IsAntialias = true,
            PathEffect = SKPathEffect.CreateDash(new float[] { 6, 5 }, 0)
        };

        using var axisPaint = new SKPaint
        {
            Color = new SKColor(140, 140, 140),
            StrokeWidth = 1,
            IsAntialias = true
        };

        for (var i = 0; i <= 5; i++)
        {
            var value = maxValue * i / 5f;
            var y = GetY(value);
            canvas.DrawLine(marginLeft, y, marginLeft + chartWidth, y, gridPaint);
            canvas.DrawText(value.ToString("0.#", CultureInfo.InvariantCulture), 18, y + 4, textPaint);
        }

        canvas.DrawLine(marginLeft, marginTop, marginLeft, marginTop + chartHeight, axisPaint);
        canvas.DrawLine(marginLeft, marginTop + chartHeight, marginLeft + chartWidth, marginTop + chartHeight, axisPaint);

        DrawSeries(canvas, primarySeries, GetX, GetY, PrimaryChart, dashed: false);
        DrawSeries(canvas, secondarySeries, GetX, GetY, SecondaryChart, dashed: true);
        DrawXAxisLabels(canvas, primarySeries.Count > 0 ? primarySeries : secondarySeries, GetX, marginTop + chartHeight + 42, textPaint);
        DrawLegend(canvas, primaryLabel, secondaryLabel, width - 310, 22);
    }

    private static void DrawSeries(
        SKCanvas canvas,
        IReadOnlyList<ChartPoint> series,
        Func<DateTime, float> getX,
        Func<float, float> getY,
        SKColor color,
        bool dashed)
    {
        if (series.Count == 0)
        {
            return;
        }

        using var linePaint = new SKPaint
        {
            Color = color,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            PathEffect = dashed ? SKPathEffect.CreateDash(new float[] { 10, 6 }, 0) : null
        };

        using var path = new SKPath();
        path.MoveTo(getX(series[0].Timestamp), getY(series[0].Value));

        for (var i = 1; i < series.Count; i++)
        {
            path.LineTo(getX(series[i].Timestamp), getY(series[i].Value));
        }

        canvas.DrawPath(path, linePaint);

        using var pointPaint = new SKPaint
        {
            Color = color,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        using var whitePaint = new SKPaint
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        foreach (var point in series)
        {
            var x = getX(point.Timestamp);
            var y = getY(point.Value);
            canvas.DrawCircle(x, y, 5, whitePaint);
            canvas.DrawCircle(x, y, 4, pointPaint);
        }
    }

    private static void DrawXAxisLabels(SKCanvas canvas, IReadOnlyList<ChartPoint> series, Func<DateTime, float> getX, float y, SKPaint textPaint)
    {
        if (series.Count == 0)
        {
            return;
        }

        var labelStep = Math.Max(1, series.Count / 5);

        for (var i = 0; i < series.Count; i += labelStep)
        {
            DrawRotatedLabel(canvas, series[i].Label, getX(series[i].Timestamp), y, textPaint);
        }

        if ((series.Count - 1) % labelStep != 0)
        {
            DrawRotatedLabel(canvas, series[^1].Label, getX(series[^1].Timestamp), y, textPaint);
        }
    }

    private static void DrawRotatedLabel(SKCanvas canvas, string label, float x, float y, SKPaint textPaint)
    {
        canvas.Save();
        canvas.RotateDegrees(-35, x, y);
        canvas.DrawText(label, x, y, textPaint);
        canvas.Restore();
    }

    private static void DrawLegend(SKCanvas canvas, string primaryLabel, string secondaryLabel, float x, float y)
    {
        using var textPaint = new SKPaint
        {
            Color = new SKColor(45, 45, 45),
            TextSize = 12,
            IsAntialias = true
        };

        using var primaryPaint = new SKPaint
        {
            Color = PrimaryChart,
            StrokeWidth = 3,
            IsAntialias = true
        };

        using var secondaryPaint = new SKPaint
        {
            Color = SecondaryChart,
            StrokeWidth = 3,
            IsAntialias = true,
            PathEffect = SKPathEffect.CreateDash(new float[] { 10, 6 }, 0)
        };

        canvas.DrawLine(x, y, x + 32, y, primaryPaint);
        canvas.DrawText(primaryLabel, x + 40, y + 4, textPaint);
        canvas.DrawLine(x, y + 20, x + 32, y + 20, secondaryPaint);
        canvas.DrawText(secondaryLabel, x + 40, y + 24, textPaint);
    }

    private static string BuildFilterLine(params (string Label, string? Value)[] filters)
    {
        var parts = filters
            .Where(f => !string.IsNullOrWhiteSpace(f.Value))
            .Select(f => $"{f.Label}: {f.Value}")
            .ToList();

        return parts.Count == 0 ? "Filtros: sin filtros" : $"Filtros: {string.Join(" | ", parts)}";
    }
}
