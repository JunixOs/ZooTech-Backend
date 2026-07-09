namespace ZooTech.Application.Common.Gateway.Export;

public sealed record ReportExportRequest(
    string Title,
    IReadOnlyList<ExportColumn> Columns,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows);
