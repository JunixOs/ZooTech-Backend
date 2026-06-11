using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZooTech.Application.Common.Models.Reports;
using ZooTech.Domain.Entities.Configuration;

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

                        inner.Item()
                            .AlignCenter()
                            .Text($"Período: {FormatearFecha(_reporte.FechaDesde)} al {FormatearFecha(_reporte.FechaHasta)}")
                            .SemiBold();

                        if (!string.IsNullOrWhiteSpace(_reporte.NombreVacuno))
                        {
                            inner.Item()
                                .AlignCenter()
                                .Text($"Vacuno: {_reporte.NombreVacuno}")
                                .SemiBold()
                                .FontColor(Colors.Grey.Darken2);
                        }
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

                            tabla.Header(header =>
                            {
                                header.Cell().Element(EstiloEncabezado)
                                    .AlignCenter().Text("Fecha");

                                header.Cell().Element(EstiloEncabezado)
                                    .AlignCenter().Text("Litros");

                                header.Cell().Element(EstiloEncabezado)
                                    .AlignCenter().Text("Ordenios");

                                header.Cell().Element(EstiloEncabezado)
                                    .AlignCenter().Text("Prom/Ord");
                            });

                            foreach (var item in _reporte.Items)
                            {
                                tabla.Cell().Element(EstiloCelda)
                                    .AlignCenter()
                                    .Text(item.Fecha.ToString("dd/MM/yyyy"));

                                tabla.Cell().Element(EstiloCelda)
                                    .AlignCenter()
                                    .Text($"{item.TotalLitros:N2}");

                                tabla.Cell().Element(EstiloCelda)
                                    .AlignCenter()
                                    .Text(item.CantidadOrdenios.ToString());

                                tabla.Cell().Element(EstiloCelda)
                                    .AlignCenter()
                                    .Text($"{(item.CantidadOrdenios > 0 ? item.TotalLitros / item.CantidadOrdenios : 0):N2}");
                            }

                            tabla.Cell().ColumnSpan(1).Element(EstiloCeldaTotal)
                                .AlignCenter()
                                .Text("TOTAL")
                                .SemiBold();

                            tabla.Cell().Element(EstiloCeldaTotal)
                                .AlignCenter()
                                .Text($"{_reporte.TotalLitrosPeriodo:N2}")
                                .SemiBold();

                            tabla.Cell().Element(EstiloCeldaTotal)
                                .AlignCenter()
                                .Text("")
                                .SemiBold();

                            tabla.Cell().Element(EstiloCeldaTotal)
                                .AlignCenter()
                                .Text("")
                                .SemiBold();
                        });
                    }
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

    private static IContainer EstiloEncabezado(IContainer container)
        => container
            .DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
            .Background(Colors.Pink.Darken2)
            .PaddingVertical(6)
            .PaddingHorizontal(5)
            .AlignMiddle();

    private static IContainer EstiloCelda(IContainer container)
        => container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(6)
            .PaddingHorizontal(5)
            .AlignMiddle();

    private static IContainer EstiloCeldaTotal(IContainer container)
        => container
            .BorderTop(1)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(7)
            .PaddingHorizontal(5)
            .AlignMiddle();

    private static string FormatearFecha(DateTime? fecha)
        => fecha.HasValue ? fecha.Value.ToString(ConfigSettings.Reporteleche.ReportFileDateFormat) : "N/A";
}
