namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

/// <summary>
/// Respuesta HTTP 201 del endpoint POST /v1/vacunos.
/// Definida en el API Contract del módulo Vacuno.
/// </summary>
public sealed class RegistrarVacunoResponse
{
    public int Id { get; set; }
    public string Codigo { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public DateOnly FechaNacimiento { get; set; }
    public string AdquisicionPor { get; set; } = default!;
    public decimal? PrecioCompra { get; set; }

    /// <summary>
    /// URL pública de la foto o null si no se adjuntó.
    /// Construida por el mapper a partir de la ruta relativa almacenada.
    /// </summary>
    public string? FotoUrl { get; set; }

    public DateTime CreadoEn { get; set; }
}