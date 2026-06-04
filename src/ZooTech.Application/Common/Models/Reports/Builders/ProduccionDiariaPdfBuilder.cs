using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZooTech.Application.Common.Models.Reports;

namespace ZooTech.Application.Common.Models.Reports.Builders;

public class ProduccionDiariaPdfBuilder : IDocument
{
    private readonly ReporteProduccionDiaria _reporte;

    public ProduccionDiariaPdfBuilder(ReporteProduccionDiaria reporte)
    {
        _reporte = reporte ?? throw new ArgumentNullException(nameof(reporte));
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header()
                .PaddingBottom(10)
                .Column(col =>
                {
                    col.Item().Text(_reporte.Metadata.Title)
                        .SemiBold()
                        .FontSize(20)
                        .FontColor(Colors.Pink.Darken2);

                    col.Item().Text($"Generado: {_reporte.Metadata.GeneratedAt:dd/MM/yyyy HH:mm:ss}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken2);
                });

            page.Content()
                .PaddingVertical(10)
                .Column(col =>
                {
                    col.Spacing(15);

                    col.Item().Column(inner =>
                    {
                        inner.Spacing(5);
                        inner.Item().Text($"Período: {FormatearFecha(_reporte.FechaDesde)} al {FormatearFecha(_reporte.FechaHasta)}")
                            .SemiBold();

                        if (!string.IsNullOrEmpty(_reporte.NombreVacuno))
                            inner.Item().Text($"Vacuno: {_reporte.NombreVacuno}");
                    });

                    if (!_reporte.Items.Any())
                    {
                        col.Item().Text("No hay datos disponibles para el período especificado")
                            .FontColor(Colors.Red.Darken2)
                            .Italic();
                    }
                    else
                    {
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                            });

                            // ✅ Usar IContainer en lugar de dynamic
                            tabla.Header(header =>
                            {
                                header.Cell().Element(EstiloEncabezado).Text("Fecha");
                                header.Cell().Element(EstiloEncabezado).Text("Litros");
                                header.Cell().Element(EstiloEncabezado).Text("Ordenios");
                                header.Cell().Element(EstiloEncabezado).Text("Prom/Ord");
                            });

                            foreach (var item in _reporte.Items)
                            {
                                tabla.Cell().Element(EstiloCelda).Text(item.Fecha.ToString("dd/MM/yyyy"));
                                tabla.Cell().Element(EstiloCelda).AlignRight().Text($"{item.TotalLitros:N2}");
                                tabla.Cell().Element(EstiloCelda).AlignRight().Text(item.CantidadOrdenios.ToString());
                                tabla.Cell().Element(EstiloCelda).AlignRight()
                                    .Text($"{(item.CantidadOrdenios > 0 ? item.TotalLitros / item.CantidadOrdenios : 0):N2}");
                            }

                            // ✅ Fila de total corregida (el span debe ir en la columna correcta)
                            tabla.Cell().ColumnSpan(3).Element(EstiloCelda)
                                .AlignRight().Text("TOTAL").SemiBold();
                            tabla.Cell().Element(EstiloCelda)
                                .AlignRight().Text($"{_reporte.TotalLitrosPeriodo:N2}").SemiBold();
                        });
                    }

                    col.Item()
                        .PaddingTop(10)
                        .BorderTop(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .PaddingTop(10)
                        .Column(inner =>
                        {
                            inner.Spacing(5);

                            inner.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Total Ordenios:");
                                row.RelativeItem().AlignRight()
                                    .Text(_reporte.TotalOrdenios.ToString())
                                    .SemiBold().FontSize(12);
                            });

                            inner.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Promedio por Ordenio:");
                                row.RelativeItem().AlignRight()
                                    .Text($"{_reporte.PromedioPorOrdenio:N2} L")
                                    .SemiBold().FontSize(12);
                            });
                        });
                });

            page.Footer()
                .AlignCenter()
                .BorderTop(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingTop(10)
                .Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
        });
    }

    // ✅ Métodos estáticos con IContainer en lugar de Action<dynamic>
    private static IContainer EstiloEncabezado(IContainer container)
        => container
            .DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
            .Background(Colors.Pink.Darken2)
            .Padding(5);

    private static IContainer EstiloCelda(IContainer container)
        => container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5);

    private static string FormatearFecha(DateTime? fecha)
        => fecha.HasValue ? fecha.Value.ToString("dd/MM/yyyy") : "N/A";
}