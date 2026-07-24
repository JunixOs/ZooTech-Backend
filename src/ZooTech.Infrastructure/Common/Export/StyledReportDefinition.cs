using ZooTech.Application.Common.Gateway.Reports;

namespace ZooTech.Infrastructure.Common.Export;

public sealed record StyledReportTheme(
    string Primary,
    string PrimaryLight,
    string TextDark,
    string BorderSoft);

public sealed record StyledReportMetadata(
    string Title,
    string SheetName,
    string FileName,
    bool IsLandscape = false);

public sealed record StyledReportSummary(string Label, string Value);

public sealed record StyledReportColumn<T>(
    string Header,
    Func<T, object?> ValueSelector,
    float RelativeWidth = 1);

public sealed record StyledTabularReport<T>(
    StyledReportMetadata Metadata,
    StyledReportTheme Theme,
    IReadOnlyCollection<T> Items,
    IReadOnlyCollection<StyledReportColumn<T>> Columns,
    IReadOnlyCollection<StyledReportSummary> Summary);

public sealed record StyledDetailField(string Label, object? Value);

public sealed record StyledDetailSection(
    string Title,
    IReadOnlyCollection<StyledDetailField> Fields);

public sealed record StyledDetailReport(
    StyledReportMetadata Metadata,
    StyledReportTheme Theme,
    IReadOnlyCollection<StyledDetailSection> Sections,
    byte[]? ImageContent = null);

public interface IStyledExcelReportRenderer
{
    Task<GeneratedReportDocument> RenderAsync<T>(
        StyledTabularReport<T> report,
        CancellationToken cancellationToken = default);

    Task<GeneratedReportDocument> RenderDetailAsync(
        StyledDetailReport report,
        CancellationToken cancellationToken = default);
}

public interface IStyledPdfReportRenderer
{
    Task<GeneratedReportDocument> RenderAsync<T>(
        StyledTabularReport<T> report,
        CancellationToken cancellationToken = default);

    Task<GeneratedReportDocument> RenderDetailAsync(
        StyledDetailReport report,
        CancellationToken cancellationToken = default);
}
