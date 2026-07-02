using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

namespace ZooTech.Infrastructure.Common.Services.PdfGenerator;

public sealed class PdfGenerateComparationService : IOrdeniosComparationPdfGeneratorService
{

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
                        .Text("Reporte de Producción de Leche")
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
                            .Canvas((canvas, size) =>
                            {
                                if (canvas is not SKCanvas skCanvas)
                                    throw new InvalidOperationException("El canvas de QuestPDF llegó nulo. No se puede dibujar el gráfico.");

                                DrawOrdeniosChart(document, skCanvas, size.Width, size.Height);
                            });

                });

            });
        }).GeneratePdf();

    }


    private void DrawOrdeniosChart(GenerateOrdeniosPdfDocument document, SKCanvas canvas, float width, float height)
    {
        canvas.Clear(SKColors.White);

        var marginLeft = 55;
        var marginRight = 25;
        var marginTop = 35;
        var marginBottom = 70;

        var chartWidth = width - marginLeft - marginRight;
        var chartHeight = height - marginTop - marginBottom;

        if (document.Items == null || document.Items.Count == 0)
        {
            using var emptyPaint = new SKPaint
            {
                Color = new SKColor(80, 80, 80),
                TextSize = 16,
                IsAntialias = true,
                FakeBoldText = true
            };

            canvas.DrawText("No hay datos de ordeños para mostrar.", marginLeft, height / 2, emptyPaint);
            return;
        }

        var data = document.Items.Select(x => new { Fecha = x.FechaHora, Litros = (float)x.Litros }).ToList();

        float minY = 0;
        float maxY = Math.Max(30, (float)Math.Ceiling(data.Max(x => x.Litros) / 5f) * 5);

        float GetX(int index, int total)
        {
            if (total == 1)
                return marginLeft + chartWidth / 2;

            return marginLeft + index * chartWidth / (total - 1);
        }

        float GetY(float value)
        {
            return marginTop + chartHeight - ((value - minY) / (maxY - minY)) * chartHeight;
        }

        using var gridPaint = new SKPaint
        {
            Color = new SKColor(210, 220, 235),
            StrokeWidth = 1,
            IsAntialias = true,
            PathEffect = SKPathEffect.CreateDash(new float[] { 6, 5 }, 0)
        };

        using var axisPaint = new SKPaint
        {
            Color = new SKColor(190, 205, 220),
            StrokeWidth = 1,
            IsAntialias = true
        };

        using var textPaint = new SKPaint
        {
            Color = new SKColor(40, 60, 90),
            TextSize = 11,
            IsAntialias = true
        };

        using var titlePaint = new SKPaint
        {
            Color = new SKColor(20, 45, 75),
            TextSize = 13,
            FakeBoldText = true,
            IsAntialias = true
        };

        canvas.DrawText("Litros (L)", marginLeft, 15, titlePaint);

        // Líneas horizontales del eje Y
        var steps = 6;

        for (int i = 0; i <= steps; i++)
        {
            float value = minY + i * (maxY - minY) / steps;
            float y = GetY(value);

            canvas.DrawLine(marginLeft, y, marginLeft + chartWidth, y, gridPaint);
            canvas.DrawText(value.ToString("0"), 25, y + 4, textPaint);
        }

        // Ejes
        canvas.DrawLine(marginLeft, marginTop, marginLeft, marginTop + chartHeight, axisPaint);
        canvas.DrawLine(marginLeft, marginTop + chartHeight, marginLeft + chartWidth, marginTop + chartHeight, axisPaint);

        // Área sombreada
        using var areaPath = new SKPath();

        areaPath.MoveTo(GetX(0, data.Count), GetY(data[0].Litros));

        for (int i = 1; i < data.Count; i++)
        {
            areaPath.LineTo(GetX(i, data.Count), GetY(data[i].Litros));
        }

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

        // Línea principal
        using var linePaint = new SKPaint
        {
            Color = new SKColor(130, 40, 80),
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };

        using var linePath = new SKPath();

        linePath.MoveTo(GetX(0, data.Count), GetY(data[0].Litros));

        for (int i = 1; i < data.Count; i++)
        {
            linePath.LineTo(GetX(i, data.Count), GetY(data[i].Litros));
        }

        canvas.DrawPath(linePath, linePaint);

        // Puntos de la línea principal
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
            float y = GetY(data[i].Litros);

            canvas.DrawCircle(x, y, 5, whitePaint);
            canvas.DrawCircle(x, y, 4, pointPaint);
        }

        // Línea de tendencia simple
        var tendencia = CalculateMovingAverage(data.Select(x => x.Litros).ToList(), 4);

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
        {
            trendPath.LineTo(GetX(i, tendencia.Count), GetY(tendencia[i]));
        }

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

        // Fechas rotadas
        var labelStep = Math.Max(1, data.Count / 6);

        for (int i = 0; i < data.Count; i += labelStep)
        {
            float x = GetX(i, data.Count);
            float y = marginTop + chartHeight + 50;

            canvas.Save();
            canvas.RotateDegrees(-90, x, y);
            canvas.DrawText(data[i].Fecha.ToString("dd/MM/yyyy"),
                            x - 30,
                            y + 4,
                            textPaint);
            canvas.Restore();
        }

        // Última fecha
        if ((data.Count - 1) % labelStep != 0)
        {
            int i = data.Count - 1;

            float x = GetX(i, data.Count);
            float y = marginTop + chartHeight + 50;

            canvas.Save();
            canvas.RotateDegrees(-90, x, y);
            canvas.DrawText(data[i].Fecha.ToString("dd/MM/yyyy"), x - 30, y + 4, textPaint);
            canvas.Restore();
        }

        canvas.DrawText("Fecha", marginLeft + chartWidth / 2 - 25, height - 10, titlePaint);
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
