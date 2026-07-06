namespace ZooTech.InterfaceAdapters.DTOs.Requests;

public sealed class ReportAnimalListRequest
{
    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public string? Keyword { get; set; }

    public string? RazaCode { get; set; }

    public string? ColorCode { get; set; }

    public string? SexoCode { get; set; }

    public string? TipoAdquisicionCode { get; set; }

    public long? GranjaId { get; set; }

    public string? EstadoCode { get; set; }
}
