using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

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
        return ComposeReport(
            title: "Reporte de ordenios",
            generatedAtUtc: document.GeneratedAtUtc,
            filterLine: BuildFilters(document),
            itemsCount: document.Items.Count,
            composeTable: c => ComposeGenericTable(
                c,
                columnWeights: new[] { 1.2f, 1.8f, 1.8f, 1.2f, 1.2f },
                headers: new[] { "Codigo", "Fecha", "Vacuno", "Litros", "Estado" },
                items: document.Items,
                cellSelector: item => new[]
                {
                    item.Codigo,
                    FormatDateTime(item.FechaHora),
                    item.NombreVacuno,
                    item.Litros.ToString("0.##", CultureInfo.InvariantCulture),
                    item.EstadoOrdenioCode
                }));
    }

    private static string BuildFilters(GenerateOrdeniosPdfDocument document)
    {
        return BuildFilterLine(
            ("VacunoId", document.VacunoId?.ToString()),
            ("Estado", document.EstadoOrdenioCode),
            ("Desde", document.FechaDesde?.ToString("yyyy-MM-dd HH:mm")),
            ("Hasta", document.FechaHasta?.ToString("yyyy-MM-dd HH:mm")));
    }

    public byte[] GenerateTriajesReport(GenerateTriajesPdfDocument document)
    {
        return ComposeReport(
            title: "Reporte de triajes",
            generatedAtUtc: document.GeneratedAtUtc,
            filterLine: BuildTriajeFilters(document),
            itemsCount: document.Items.Count,
            composeTable: c => ComposeGenericTable(
                c,
                columnWeights: new[] { 1.2f, 1.5f, 1f, 1.8f, 1.2f, 1f, 2f },
                headers: new[] { "C.Registro", "Fecha", "Hora", "Vacuno", "Tipo Peso", "Peso (Kg)", "Observaciones" },
                items: document.Items,
                cellSelector: item => new[]
                {
                    item.Codigo,
                    FormatDate(item.FechaHora),
                    FormatTime(item.FechaHora),
                    item.VacunoNombre,
                    item.TipoPesoCode,
                    item.PesoKg.ToString("0.##", CultureInfo.InvariantCulture),
                    item.Observaciones ?? string.Empty
                }));
    }

    private static string BuildTriajeFilters(GenerateTriajesPdfDocument document)
    {
        return BuildFilterLine(
            ("Fecha", document.Fecha),
            ("Código", document.Codigo),
            ("Nombre", document.Nombre),
            ("Tipo peso", document.TipoPeso),
            ("Peso", !string.IsNullOrWhiteSpace(document.PesoKg) ? $"{document.PesoKg} Kg" : null));
    }

    private static byte[] ComposeReport(
        string title,
        DateTime generatedAtUtc,
        string filterLine,
        int itemsCount,
        Action<IContainer> composeTable)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(24);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(TextDark));

                page.Header().Element(c => ComposeHeader(c, title, generatedAtUtc, filterLine));

                page.Content().PaddingTop(16).Column(column =>
                {
                    if (itemsCount == 0)
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

                    column.Item().Element(composeTable);
                });

                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, string title, DateTime generatedAtUtc, string filterLine)
    {
        container
            .Background(Primary)
            .Padding(16)
            .Column(column =>
            {
                column.Item().Text(title)
                    .FontColor(Colors.White)
                    .SemiBold()
                    .FontSize(18);

                column.Item().PaddingTop(4)
                    .Text($"Generado: {FormatDateTime(generatedAtUtc)} UTC")
                    .FontColor(Colors.White)
                    .FontSize(9);

                column.Item().PaddingTop(2)
                    .Text(filterLine)
                    .FontColor(Colors.White)
                    .FontSize(9);
            });
    }

    private static void ComposeGenericTable<T>(
        IContainer container,
        float[] columnWeights,
        string[] headers,
        IReadOnlyList<T> items,
        Func<T, string[]> cellSelector)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                foreach (var weight in columnWeights)
                {
                    columns.RelativeColumn(weight);
                }
            });

            table.Header(header =>
            {
                foreach (var title in headers)
                {
                    header.Cell().Element(StyleHeaderCell).Text(title);
                }
            });

            for (var i = 0; i < items.Count; i++)
            {
                var values = cellSelector(items[i]);
                var isAlternate = i % 2 == 1;

                foreach (var value in values)
                {
                    table.Cell().Element(x => StyleRowCell(x, isAlternate)).Text(value);
                }
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

    private static string BuildFilterLine(params (string Label, string? Value)[] filters)
    {
        var parts = filters
            .Where(f => !string.IsNullOrWhiteSpace(f.Value))
            .Select(f => $"{f.Label}: {f.Value}")
            .ToList();

        return parts.Count == 0 ? "Filtros: sin filtros" : $"Filtros: {string.Join(" | ", parts)}";
    }

    private static string FormatDate(DateTime value)
        => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static string FormatTime(DateTime value)
        => value.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

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
