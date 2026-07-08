namespace ZooTech.InterfaceAdapters.DTOs.Responses;

public sealed class ReportAnimalListResponse
{
    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public string? Keyword { get; set; }

    public int Total { get; set; }

    public IReadOnlyCollection<ReportAnimalListItemResponse> Items { get; set; } = [];
}
