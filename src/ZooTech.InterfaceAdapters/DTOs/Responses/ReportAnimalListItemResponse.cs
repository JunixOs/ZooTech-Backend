namespace ZooTech.InterfaceAdapters.DTOs.Responses;

public sealed class ReportAnimalListItemResponse
{
    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Raza { get; set; } = string.Empty;

    public string Sexo { get; set; } = string.Empty;

    public string Procedencia { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public DateOnly FechaRegistro { get; set; }
}
