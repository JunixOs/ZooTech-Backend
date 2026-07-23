using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Common.Gateway.Time;

namespace ZooTech.Infrastructure.Common.Export;

public sealed class StyledPdfReportRenderer : IStyledPdfReportRenderer
{
    private readonly ITenantReportBrandingProvider _brandingProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public StyledPdfReportRenderer(
        ITenantReportBrandingProvider brandingProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _brandingProvider = brandingProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GeneratedReportDocument> RenderAsync<T>(
        StyledTabularReport<T> report,
        CancellationToken cancellationToken = default)
    {
        var branding = await _brandingProvider.GetAsync(cancellationToken);
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePage(page, report.Metadata, report.Theme);
                page.Header().Element(header => ComposeHeader(
                    header,
                    report.Metadata,
                    report.Theme,
                    branding));
                page.Content().PaddingVertical(12).Column(column =>
                {
                    column.Spacing(8);
                    if (report.Summary.Count > 0)
                    {
                        column.Item().Element(summary => ComposeSummary(
                            summary,
                            report.Summary,
                            report.Theme));
                    }

                    if (report.Items.Count == 0)
                    {
                        column.Item()
                            .Border(1)
                            .BorderColor(report.Theme.BorderSoft)
                            .Padding(12)
                            .AlignCenter()
                            .Text("No se encontraron registros.");
                    }
                    else
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var reportColumn in report.Columns)
                                {
                                    columns.RelativeColumn(reportColumn.RelativeWidth);
                                }
                            });

                            table.Header(header =>
                            {
                                foreach (var reportColumn in report.Columns)
                                {
                                    header.Cell()
                                        .Element(cell => HeaderCell(cell, report.Theme))
                                        .Text(reportColumn.Header);
                                }
                            });

                            var itemIndex = 0;
                            foreach (var item in report.Items)
                            {
                                foreach (var reportColumn in report.Columns)
                                {
                                    table.Cell()
                                        .Element(cell => DataCell(
                                            cell,
                                            report.Theme,
                                            itemIndex % 2 == 1))
                                        .Text(StyledReportValueFormatter.Format(
                                            reportColumn.ValueSelector(item)));
                                }

                                itemIndex++;
                            }
                        });
                    }
                });
                page.Footer().Element(footer => ComposeFooter(footer, report.Theme));
            });
        });

        return new GeneratedReportDocument(
            document.GeneratePdf(),
            "application/pdf",
            ".pdf",
            report.Metadata.FileName);
    }

    public async Task<GeneratedReportDocument> RenderDetailAsync(
        StyledDetailReport report,
        CancellationToken cancellationToken = default)
    {
        var branding = await _brandingProvider.GetAsync(cancellationToken);
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePage(page, report.Metadata, report.Theme);
                page.Header().Element(header => ComposeHeader(
                    header,
                    report.Metadata,
                    report.Theme,
                    branding));
                page.Content().PaddingVertical(12).Column(column =>
                {
                    column.Spacing(12);
                    if (report.ImageContent is not null)
                    {
                        column.Item()
                            .Height(140)
                            .Border(1)
                            .BorderColor(report.Theme.BorderSoft)
                            .Padding(5)
                            .Image(report.ImageContent)
                            .FitArea();
                    }

                    foreach (var section in report.Sections)
                    {
                        column.Item().Element(container =>
                            ComposeDetailSection(container, section, report.Theme));
                    }
                });
                page.Footer().Element(footer => ComposeFooter(footer, report.Theme));
            });
        });

        return new GeneratedReportDocument(
            document.GeneratePdf(),
            "application/pdf",
            ".pdf",
            report.Metadata.FileName);
    }

    private void ComposeHeader(
        IContainer container,
        StyledReportMetadata metadata,
        StyledReportTheme theme,
        TenantReportBranding branding)
    {
        container.Background(theme.Primary).Padding(14).Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(branding.DisplayName)
                    .Bold()
                    .FontSize(18)
                    .FontColor(Colors.White);
                column.Item().Text(metadata.Title)
                    .SemiBold()
                    .FontSize(12)
                    .FontColor(theme.PrimaryLight);
                column.Item().Text($"Generado: {_dateTimeProvider.ServerNow:yyyy-MM-dd HH:mm:ss} UTC")
                    .FontSize(8)
                    .FontColor(theme.PrimaryLight);
            });

            if (branding.LogoContent is not null)
            {
                row.ConstantItem(48).Height(48).Image(branding.LogoContent).FitArea();
            }
        });
    }

    private static void ConfigurePage(
        PageDescriptor page,
        StyledReportMetadata metadata,
        StyledReportTheme theme)
    {
        page.Size(metadata.IsLandscape ? PageSizes.A4.Landscape() : PageSizes.A4);
        page.Margin(1.3f, Unit.Centimetre);
        page.DefaultTextStyle(style => style.FontSize(9).FontColor(theme.TextDark));
    }

    private static void ComposeSummary(
        IContainer container,
        IReadOnlyCollection<StyledReportSummary> summary,
        StyledReportTheme theme)
    {
        container.Background(theme.PrimaryLight).Padding(8).Column(column =>
        {
            foreach (var item in summary)
            {
                column.Item().Text(text =>
                {
                    text.Span($"{item.Label}: ").SemiBold();
                    text.Span(item.Value);
                });
            }
        });
    }

    private static void ComposeDetailSection(
        IContainer container,
        StyledDetailSection section,
        StyledReportTheme theme)
    {
        container.Column(column =>
        {
            column.Item()
                .Background(theme.PrimaryLight)
                .Padding(7)
                .Text(section.Title)
                .Bold()
                .FontSize(11);

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                foreach (var field in section.Fields)
                {
                    table.Cell()
                        .Element(cell => HeaderCell(cell, theme))
                        .Text(field.Label);
                    table.Cell()
                        .Element(cell => DataCell(cell, theme, false))
                        .Text(string.IsNullOrWhiteSpace(
                            StyledReportValueFormatter.Format(field.Value))
                                ? "-"
                                : StyledReportValueFormatter.Format(field.Value));
                }
            });
        });
    }

    private static void ComposeFooter(IContainer container, StyledReportTheme theme)
    {
        container
            .BorderTop(1)
            .BorderColor(theme.BorderSoft)
            .PaddingTop(6)
            .Row(row =>
            {
                row.RelativeItem().Text("ZooTech").FontSize(8);
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
    }

    private static IContainer HeaderCell(IContainer container, StyledReportTheme theme)
        => container
            .Border(1)
            .BorderColor(theme.BorderSoft)
            .Background(theme.PrimaryLight)
            .PaddingVertical(4)
            .PaddingHorizontal(5)
            .DefaultTextStyle(style => style.Bold().FontColor(theme.TextDark));

    private static IContainer DataCell(
        IContainer container,
        StyledReportTheme theme,
        bool alternate)
        => container
            .Border(1)
            .BorderColor(theme.BorderSoft)
            .Background(alternate ? theme.PrimaryLight : Colors.White)
            .PaddingVertical(4)
            .PaddingHorizontal(5);
}
