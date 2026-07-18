using System;

namespace ZooTech.Application.Modules.Module_Vacuno.Common;

public sealed class VacunoDetalleDto
{
    public long Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateOnly FechaNacimiento { get; set; }
    public string AdquisicionPor { get; set; } = string.Empty;
    public decimal? PrecioCompra { get; set; }
    public string Raza { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Sexo { get; set; } = string.Empty;
    public string? CodigoPadre { get; set; }
    public string? CodigoMadre { get; set; }
    public string? Granja { get; set; }
    public string? Distrito { get; set; }
    public string? Provincia { get; set; }
    public string? Departamento { get; set; }
    public string? CodigoDistrito { get; set; }
    public string? AptoPara { get; set; }
    public DateOnly? FechaEspecificacion { get; set; }
    public string? Observaciones { get; set; }
    public string? FotoUrl { get; set; }
    public string Estado { get; set; } = "SANO";
    public DateTime CreadoEn { get; set; }
    public DateTime ActualizadoEn { get; set; }
    
    // Propiedades adicionales útiles para persistencia interna si fuera necesario
    public long? PadreId { get; set; }
    public long? MadreId { get; set; }
    public long GranjaId { get; set; }
}
