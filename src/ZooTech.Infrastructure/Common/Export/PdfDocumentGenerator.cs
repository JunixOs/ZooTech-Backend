using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZooTech.Application.Common.Gateway.Export;

namespace ZooTech.Infrastructure.Common.Export;

public class PdfDocumentGenerator : IPdfDocumentGenerator
{
    private const int LandscapeColumnThreshold = 6;

    public byte[] Generate(ReportExportRequest request)
    {
        var isLandscape = request.Columns.Count > LandscapeColumnThreshold;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(isLandscape ? PageSizes.A4.Landscape() : PageSizes.A4);
                page.DefaultTextStyle(style => style.FontSize(9));

                page.Header()
                    .Text(request.Title)
                    .FontSize(16)
                    .Bold();

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var _ in request.Columns)
                        {
                            columns.RelativeColumn();
                        }
                    });

                    table.Header(header =>
                    {
                        foreach (var column in request.Columns)
                        {
                            header.Cell()
                                .Background(Colors.Grey.Lighten2)
                                .Padding(4)
                                .Text(column.Header)
                                .Bold();
                        }
                    });

                    foreach (var row in request.Rows)
                    {
                        foreach (var column in request.Columns)
                        {
                            var value = row.TryGetValue(column.Key, out var cellValue) && cellValue is not null
                                ? cellValue.ToString()
                                : string.Empty;

                            table.Cell()
                                .Padding(4)
                                .Text(value ?? string.Empty);
                        }
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }
}
