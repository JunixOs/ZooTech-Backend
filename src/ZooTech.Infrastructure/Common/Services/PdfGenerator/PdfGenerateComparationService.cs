using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

namespace ZooTech.Infrastructure.Common.Services.PdfGenerator;

public sealed class PdfGenerateComparationService : IOrdeniosComparationPdfGeneratorService
{
    private sealed record ChartPoint(string Label, float Value);

    static PdfGenerateComparationService()
    {
        QuestPDF.Settings.License = LicenseType.Community;

    }

    private static readonly Color Primary = Color.FromHex("#802b4d");
    private static readonly Color PrimaryDark = Color.FromHex("#66223e");
    private static readonly Color PrimaryLight = Color.FromHex("#ecdfe4");   // zebra rows
    private static readonly Color PrimaryLighter = Color.FromHex("#f9f4f6"); // fondos suaves
    public byte[] GenerateOrdeniosReport(GenerateOrdeniosPdfDocument document)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {


                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content().Column(column =>
                {
                    column.Item()
                        .Text(GetReportTitle(document))
                        .FontSize(18)
                        .Bold()
                        .AlignCenter();

                    column.Item().Height(20);

                     column.Item()
                            .Border(1)
                            .BorderColor(Primary)
                             .Background(PrimaryLight)
                             .Padding(15)
                             .Height(360)
                             .Svg(size => BuildChartSvg(document, size.Width, size.Height));

                });

            });
        }).GeneratePdf();

    }

    private static string GetReportTitle(GenerateOrdeniosPdfDocument document)
        => document.VacunoId.HasValue
            ? $"Comparativo de producción del vacuno {document.VacunoId.Value}"
            : "Comparativo de producción por vacunos";

    private static string BuildChartSvg(GenerateOrdeniosPdfDocument document, float width, float height)
    {
        using var stream = new MemoryStream();
        using (var canvas = SKSvgCanvas.Create(new SKRect(0, 0, width, height), stream))
        {
            DrawOrdeniosChart(document, canvas, width, height);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }


    private static void DrawOrdeniosChart(GenerateOrdeniosPdfDocument document, SKCanvas canvas, float width, float height)
    {
        canvas.Clear(SKColors.White);

        if (document.VacunoId.HasValue)
        {
            DrawVacunoHistoryChart(document, canvas, width, height);
            return;
        }

        DrawVacunosComparisonChart(document, canvas, width, height);
    }

    private static void DrawVacunoHistoryChart(GenerateOrdeniosPdfDocument document, SKCanvas canvas, float width, float height)
    {
        var data = document.Items
            .OrderBy(x => x.FechaHora)
            .Select(x => new ChartPoint(x.FechaHora.ToString("dd/MM/yyyy"), (float)x.Litros))
            .ToList();

        DrawLineChart(
            canvas,
            width,
            height,
            data,
            emptyMessage: "No hay datos de ordeños para mostrar.",
            xAxisLabel: "Fecha",
            yAxisLabel: "Litros (L)");
    }

    private static void DrawVacunosComparisonChart(GenerateOrdeniosPdfDocument document, SKCanvas canvas, float width, float height)
    {
        var data = document.Items
            .GroupBy(x => new { x.VacunoId, x.NombreVacuno })
            .Select(group => new ChartPoint(group.Key.NombreVacuno, (float)group.Sum(x => x.Litros)))
            .OrderByDescending(x => x.Value)
            .ToList();

        DrawBarChart(
            canvas,
            width,
            height,
            data,
            emptyMessage: "No hay vacunos para comparar.",
            xAxisLabel: "Vacunos",
            yAxisLabel: "Litros (L)");
    }

    private static void DrawLineChart(SKCanvas canvas, float width, float height, List<ChartPoint> data, string emptyMessage, string xAxisLabel, string yAxisLabel)
    {
        var marginLeft = 55;
        var marginRight = 25;
        var marginTop = 35;
        var marginBottom = 70;

        var chartWidth = width - marginLeft - marginRight;
        var chartHeight = height - marginTop - marginBottom;

        if (data.Count == 0)
        {
            using var emptyPaint = new SKPaint
            {
                Color = new SKColor(80, 80, 80),
                TextSize = 16,
                IsAntialias = true,
                FakeBoldText = true
            };

            canvas.DrawText(emptyMessage, marginLeft, height / 2, emptyPaint);
            return;
        }

        float minY = 0;
        float maxY = Math.Max(30, (float)Math.Ceiling(data.Max(x => x.Value) / 5f) * 5);

        float GetX(int index, int total)
        {
            if (total == 1)
                return marginLeft + chartWidth / 2;

            return marginLeft + index * chartWidth / (total - 1);
        }

        float GetY(float value)
            => marginTop + chartHeight - ((value - minY) / (maxY - minY)) * chartHeight;

        CreatePaints(out var gridPaint, out var axisPaint, out var textPaint, out var titlePaint);
        using (gridPaint)
        using (axisPaint)
        using (textPaint)
        using (titlePaint)
        {
            canvas.DrawText(yAxisLabel, marginLeft, 15, titlePaint);

            var steps = 6;

            for (int i = 0; i <= steps; i++)
            {
                float value = minY + i * (maxY - minY) / steps;
                float y = GetY(value);

                canvas.DrawLine(marginLeft, y, marginLeft + chartWidth, y, gridPaint);
                canvas.DrawText(value.ToString("0"), 25, y + 4, textPaint);
            }

            canvas.DrawLine(marginLeft, marginTop, marginLeft, marginTop + chartHeight, axisPaint);
            canvas.DrawLine(marginLeft, marginTop + chartHeight, marginLeft + chartWidth, marginTop + chartHeight, axisPaint);

            using var areaPath = new SKPath();
            areaPath.MoveTo(GetX(0, data.Count), GetY(data[0].Value));

            for (int i = 1; i < data.Count; i++)
                areaPath.LineTo(GetX(i, data.Count), GetY(data[i].Value));

            areaPath.LineTo(GetX(data.Count - 1, data.Count), GetY(0));
            areaPath.LineTo(GetX(0, data.Count), GetY(0));
            areaPath.Close();

            using var areaPaint = new SKPaint
            {
                Color = new SKColor(130, 40, 80, 45),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            canvas.DrawPath(areaPath, areaPaint);

            using var linePaint = new SKPaint
            {
                Color = new SKColor(130, 40, 80),
                StrokeWidth = 3,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            using var linePath = new SKPath();
            linePath.MoveTo(GetX(0, data.Count), GetY(data[0].Value));

            for (int i = 1; i < data.Count; i++)
                linePath.LineTo(GetX(i, data.Count), GetY(data[i].Value));

            canvas.DrawPath(linePath, linePaint);

            using var pointPaint = new SKPaint
            {
                Color = new SKColor(130, 40, 80),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            using var whitePaint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            for (int i = 0; i < data.Count; i++)
            {
                float x = GetX(i, data.Count);
                float y = GetY(data[i].Value);

                canvas.DrawCircle(x, y, 5, whitePaint);
                canvas.DrawCircle(x, y, 4, pointPaint);
            }

            var tendencia = CalculateMovingAverage(data.Select(x => x.Value).ToList(), 4);

            using var trendPaint = new SKPaint
            {
                Color = new SKColor(105, 35, 185),
                StrokeWidth = 3,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new float[] { 10, 5 }, 0)
            };

            using var trendPath = new SKPath();
            trendPath.MoveTo(GetX(0, tendencia.Count), GetY(tendencia[0]));

            for (int i = 1; i < tendencia.Count; i++)
                trendPath.LineTo(GetX(i, tendencia.Count), GetY(tendencia[i]));

            canvas.DrawPath(trendPath, trendPaint);

            using var trendPointPaint = new SKPaint
            {
                Color = new SKColor(105, 35, 185),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            for (int i = 0; i < tendencia.Count; i++)
            {
                float x = GetX(i, tendencia.Count);
                float y = GetY(tendencia[i]);

                canvas.DrawCircle(x, y, 5, whitePaint);
                canvas.DrawCircle(x, y, 4, trendPointPaint);
            }

            var labelStep = Math.Max(1, data.Count / 6);

            for (int i = 0; i < data.Count; i += labelStep)
                DrawRotatedLabel(canvas, data[i].Label, GetX(i, data.Count), marginTop + chartHeight + 50, textPaint);

            if ((data.Count - 1) % labelStep != 0)
                DrawRotatedLabel(canvas, data[^1].Label, GetX(data.Count - 1, data.Count), marginTop + chartHeight + 50, textPaint);

            canvas.DrawText(xAxisLabel, marginLeft + chartWidth / 2 - 25, height - 10, titlePaint);
        }
    }

    private static void DrawBarChart(SKCanvas canvas, float width, float height, List<ChartPoint> data, string emptyMessage, string xAxisLabel, string yAxisLabel)
    {
        var marginLeft = 55;
        var marginRight = 25;
        var marginTop = 35;
        var marginBottom = 95;

        var chartWidth = width - marginLeft - marginRight;
        var chartHeight = height - marginTop - marginBottom;

        if (data.Count == 0)
        {
            using var emptyPaint = new SKPaint
            {
                Color = new SKColor(80, 80, 80),
                TextSize = 16,
                IsAntialias = true,
                FakeBoldText = true
            };

            canvas.DrawText(emptyMessage, marginLeft, height / 2, emptyPaint);
            return;
        }

        float minY = 0;
        float maxY = Math.Max(30, (float)Math.Ceiling(data.Max(x => x.Value) / 5f) * 5);

        float GetY(float value)
            => marginTop + chartHeight - ((value - minY) / (maxY - minY)) * chartHeight;

        CreatePaints(out var gridPaint, out var axisPaint, out var textPaint, out var titlePaint);
        using (gridPaint)
        using (axisPaint)
        using (textPaint)
        using (titlePaint)
        {
            canvas.DrawText(yAxisLabel, marginLeft, 15, titlePaint);

            var steps = 6;
            for (int i = 0; i <= steps; i++)
            {
                float value = minY + i * (maxY - minY) / steps;
                float y = GetY(value);

                canvas.DrawLine(marginLeft, y, marginLeft + chartWidth, y, gridPaint);
                canvas.DrawText(value.ToString("0"), 25, y + 4, textPaint);
            }

            canvas.DrawLine(marginLeft, marginTop, marginLeft, marginTop + chartHeight, axisPaint);
            canvas.DrawLine(marginLeft, marginTop + chartHeight, marginLeft + chartWidth, marginTop + chartHeight, axisPaint);

            using var barPaint = new SKPaint
            {
                Color = new SKColor(130, 40, 80),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            using var barShadowPaint = new SKPaint
            {
                Color = new SKColor(130, 40, 80, 45),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            var slotWidth = chartWidth / data.Count;
            var barWidth = Math.Max(18, slotWidth * 0.55f);

            for (int i = 0; i < data.Count; i++)
            {
                var centerX = marginLeft + (slotWidth * i) + (slotWidth / 2);
                var left = centerX - (barWidth / 2);
                var top = GetY(data[i].Value);
                var rect = new SKRect(left, top, left + barWidth, marginTop + chartHeight);

                canvas.DrawRect(new SKRect(rect.Left + 3, rect.Top + 3, rect.Right + 3, rect.Bottom), barShadowPaint);
                canvas.DrawRect(rect, barPaint);
                canvas.DrawText(data[i].Value.ToString("0.##"), left - 2, top - 6, textPaint);
                DrawRotatedLabel(canvas, TruncateLabel(data[i].Label), centerX, marginTop + chartHeight + 55, textPaint);
            }

            canvas.DrawText(xAxisLabel, marginLeft + chartWidth / 2 - 35, height - 10, titlePaint);
        }
    }

    private static void DrawRotatedLabel(SKCanvas canvas, string label, float x, float y, SKPaint textPaint)
    {
        canvas.Save();
        canvas.RotateDegrees(-90, x, y);
        canvas.DrawText(label, x - 30, y + 4, textPaint);
        canvas.Restore();
    }

    private static string TruncateLabel(string label)
        => label.Length <= 12 ? label : label[..12];

    private static void CreatePaints(out SKPaint gridPaint, out SKPaint axisPaint, out SKPaint textPaint, out SKPaint titlePaint)
    {
        gridPaint = new SKPaint
        {
            Color = new SKColor(210, 220, 235),
            StrokeWidth = 1,
            IsAntialias = true,
            PathEffect = SKPathEffect.CreateDash(new float[] { 6, 5 }, 0)
        };

        axisPaint = new SKPaint
        {
            Color = new SKColor(190, 205, 220),
            StrokeWidth = 1,
            IsAntialias = true
        };

        textPaint = new SKPaint
        {
            Color = new SKColor(40, 60, 90),
            TextSize = 11,
            IsAntialias = true
        };

        titlePaint = new SKPaint
        {
            Color = new SKColor(20, 45, 75),
            TextSize = 13,
            FakeBoldText = true,
            IsAntialias = true
        };
    }

    private static List<float> CalculateMovingAverage(List<float> values, int window)
    {
        var result = new List<float>();

        for (int i = 0; i < values.Count; i++)
        {
            int start = Math.Max(0, i - window + 1);
            var range = values.Skip(start).Take(i - start + 1);
            result.Add(range.Average());
        }

        return result;
    }


}
