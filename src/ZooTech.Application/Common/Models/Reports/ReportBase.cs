namespace ZooTech.Application.Common.Models.Reports;

/// <summary>
/// Metadatos comunes para todos los reportes
/// </summary>
public record ReportMetadata
{
    public string Title { get; init; } = string.Empty;
    public DateTime GeneratedAt { get; init; } = DateTime.Now;
    public string? GeneratedBy { get; init; }
    public string? Company { get; init; }
    public string? Department { get; init; }
}

/// <summary>
/// Clase base para todos los reportes
/// </summary>
public abstract class ReportBase
{
    public ReportMetadata Metadata { get; set; } = new();
}
