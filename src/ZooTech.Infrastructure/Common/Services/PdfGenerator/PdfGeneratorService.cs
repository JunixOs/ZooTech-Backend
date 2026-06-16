using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

namespace ZooTech.Infrastructure.Common.Services.PdfGenerator;

public sealed class PdfGeneratorService : IPdfGeneratorService
{
    static PdfGeneratorService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateOrdeniosReport(GenerateOrdeniosPdfDocument document)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(24);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Text("Reporte de ordenios").SemiBold().FontSize(18);
                    column.Item().Text($"Generado: {FormatDateTime(document.GeneratedAtUtc)} UTC");
                    column.Item().Text(BuildFilters(document));
                });

                page.Content().PaddingVertical(12).Column(column =>
                {
                    if (document.Items.Count == 0)
                    {
                        column.Item().Text("No se encontraron registros para los filtros solicitados.");
                        return;
                    }

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.8f);
                            columns.RelativeColumn(1.8f);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.2f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(x => StyleCell(x, true)).Text("Codigo");
                            header.Cell().Element(x => StyleCell(x, true)).Text("Fecha");
                            header.Cell().Element(x => StyleCell(x, true)).Text("Vacuno");
                            header.Cell().Element(x => StyleCell(x, true)).Text("Litros");
                            header.Cell().Element(x => StyleCell(x, true)).Text("Estado");
                        });

                        foreach (var item in document.Items)
                        {
                            table.Cell().Element(x => StyleCell(x, false)).Text(item.Codigo);
                            table.Cell().Element(x => StyleCell(x, false)).Text(FormatDateTime(item.FechaHora));
                            table.Cell().Element(x => StyleCell(x, false)).Text(item.NombreVacuno);
                            table.Cell().Element(x => StyleCell(x, false)).Text(item.Litros.ToString("0.##", CultureInfo.InvariantCulture));
                            table.Cell().Element(x => StyleCell(x, false)).Text(item.EstadoOrdenioCode);
                        }
                    });
                });

                page.Footer().AlignRight().Text(text =>
                {
                    text.Span("Pagina ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static string BuildFilters(GenerateOrdeniosPdfDocument document)
    {
        var filters = new List<string>();

        if (document.VacunoId.HasValue)
            filters.Add($"VacunoId: {document.VacunoId.Value}");

        if (!string.IsNullOrWhiteSpace(document.EstadoOrdenioCode))
            filters.Add($"Estado: {document.EstadoOrdenioCode}");

        if (document.FechaDesde.HasValue)
            filters.Add($"Desde: {document.FechaDesde.Value:yyyy-MM-dd HH:mm}");

        if (document.FechaHasta.HasValue)
            filters.Add($"Hasta: {document.FechaHasta.Value:yyyy-MM-dd HH:mm}");

        return filters.Count == 0 ? "Filtros: sin filtros" : $"Filtros: {string.Join(" | ", filters)}";
    }

    private static string FormatDateTime(DateTime value)
        => value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

    private static IContainer StyleCell(IContainer container, bool header)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(6)
            .Background(header ? Colors.Grey.Lighten3 : Colors.White);
    }
}
