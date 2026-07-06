namespace ZooTech.InterfaceAdapters.DTOs.Responses;

public sealed class ReportAnimalListItemResponse
{
    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public DateOnly FechaNacimiento { get; set; }

    public string TipoAdquisicion { get; set; } = string.Empty;

    public string Raza { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string Sexo { get; set; } = string.Empty;

    public string Granja { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public DateOnly FechaRegistro { get; set; }
}
