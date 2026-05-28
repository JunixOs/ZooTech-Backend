namespace ZooTech.InterfaceAdapters.DTOs.Requests;

public sealed class ReportAnimalListRequest
{
    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public string? Keyword { get; set; }
}
