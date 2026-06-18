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

    // Paleta basada en #802b4d
    private static readonly Color Primary = Color.FromHex("#802b4d");
    private static readonly Color PrimaryDark = Color.FromHex("#66223e");
    private static readonly Color PrimaryLight = Color.FromHex("#ecdfe4");   // zebra rows
    private static readonly Color PrimaryLighter = Color.FromHex("#f9f4f6"); // fondos suaves
    private static readonly Color BorderSoft = Color.FromHex("#d9bfca");
    private static readonly Color TextDark = Color.FromHex("#2d2d2d");
    private static readonly Color TextMuted = Color.FromHex("#767676");

    public byte[] GenerateOrdeniosReport(GenerateOrdeniosPdfDocument document)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(24);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(TextDark));

                page.Header().Element(c => ComposeHeader(c, document));

                page.Content().PaddingTop(16).Column(column =>
                {
                    if (document.Items.Count == 0)
                    {
                        column.Item()
                            .Background(PrimaryLighter)
                            .Border(1)
                            .BorderColor(BorderSoft)
                            .Padding(16)
                            .AlignCenter()
                            .Text("No se encontraron registros para los filtros solicitados.")
                            .FontColor(TextMuted);
                        return;
                    }

                    column.Item().Element(c => ComposeTable(c, document));
                });

                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, GenerateOrdeniosPdfDocument document)
    {
        container
            .Background(Primary)
            .Padding(16)
            .Column(column =>
            {
                column.Item().Text("Reporte de ordenios")
                    .FontColor(Colors.White)
                    .SemiBold()
                    .FontSize(18);

                column.Item().PaddingTop(4)
                    .Text($"Generado: {FormatDateTime(document.GeneratedAtUtc)} UTC")
                    .FontColor(Colors.White)
                    .FontSize(9);

                column.Item().PaddingTop(2)
                    .Text(BuildFilters(document))
                    .FontColor(Colors.White)
                    .FontSize(9);
            });
    }

    private static void ComposeTable(IContainer container, GenerateOrdeniosPdfDocument document)
    {
        container.Table(table =>
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
                header.Cell().Element(StyleHeaderCell).Text("Codigo");
                header.Cell().Element(StyleHeaderCell).Text("Fecha");
                header.Cell().Element(StyleHeaderCell).Text("Vacuno");
                header.Cell().Element(StyleHeaderCell).Text("Litros");
                header.Cell().Element(StyleHeaderCell).Text("Estado");
            });

            for (var i = 0; i < document.Items.Count; i++)
            {
                var item = document.Items[i];
                var isAlternate = i % 2 == 1;

                table.Cell().Element(x => StyleRowCell(x, isAlternate)).Text(item.Codigo);
                table.Cell().Element(x => StyleRowCell(x, isAlternate)).Text(FormatDateTime(item.FechaHora));
                table.Cell().Element(x => StyleRowCell(x, isAlternate)).Text(item.NombreVacuno);
                table.Cell().Element(x => StyleRowCell(x, isAlternate)).Text(item.Litros.ToString("0.##", CultureInfo.InvariantCulture));
                table.Cell().Element(x => StyleRowCell(x, isAlternate)).Text(item.EstadoOrdenioCode);
            }
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container
            .BorderTop(1)
            .BorderColor(BorderSoft)
            .PaddingTop(6)
            .Row(row =>
            {
                row.RelativeItem().Text("ZooTech").FontColor(TextMuted).FontSize(8);

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontColor(TextMuted).FontSize(8));
                    text.Span("Pagina ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
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

    private static IContainer StyleHeaderCell(IContainer container)
    {
        return container
            .Background(PrimaryDark)
            .Padding(8)
            .DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold());
    }

    private static IContainer StyleRowCell(IContainer container, bool isAlternate)
    {
        return container
            .Border(1)
            .BorderColor(BorderSoft)
            .Background(isAlternate ? PrimaryLight : Colors.White)
            .Padding(6);
    }
}