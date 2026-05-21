using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.ReadModels;

[Keyless]
public sealed class RegistroVacunoReporteRow
{
    public long Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateOnly FechaNacimiento { get; set; }
    public string? Sexo { get; set; }
    public string? Raza { get; set; }
    public string? Color { get; set; }
    public string? AdquisicionPor { get; set; }
    public decimal? PrecioCompra { get; set; }
    public string? CodigoPadre { get; set; }
    public string? CodigoMadre { get; set; }
    public string? CodigoAbuelo { get; set; }
    public string? CodigoAbuela { get; set; }
    public string? Granja { get; set; }
    public string? Distrito { get; set; }
    public string? Departamento { get; set; }
    public string? Provincia { get; set; }
    public string? Procedencia { get; set; }
    public long? FotoId { get; set; }
    public string? FotoNombreOriginal { get; set; }
    public string? FotoNombreAlmacenado { get; set; }
    public string? FotoRuta { get; set; }
    public string? FotoUrl { get; set; }
    public string? FotoExtension { get; set; }
    public long? FotoTamanoBytes { get; set; }
    public string? Observaciones { get; set; }
    public string? EstadoActualCode { get; set; }
    public string? EstadoActualNombre { get; set; }
    public string? Estado { get; set; }
    public DateOnly? FechaEstado { get; set; }
    public string? MotivoEstado { get; set; }
    public string? AptoPara { get; set; }
    public DateOnly FechaRegistro { get; set; }
    public DateOnly? FechaAdquisicion { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public DateTime ActualizadoEn { get; set; }
}
