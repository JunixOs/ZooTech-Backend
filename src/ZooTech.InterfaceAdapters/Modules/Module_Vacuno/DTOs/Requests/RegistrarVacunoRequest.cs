using Microsoft.AspNetCore.Http;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

public sealed class RegistrarVacunoRequest
{
    public string Codigo { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public DateOnly FechaNacimiento { get; set; }

    public int IdTipoAdquisicion { get; set; }
    public decimal? PrecioCompra { get; set; }

    public int IdRaza { get; set; }
    public int IdColor { get; set; }
    public int IdSexo { get; set; }

    public string CodigoPadre { get; set; } = default!;
    public string CodigoMadre { get; set; } = default!;

    public string NombreGranja { get; set; } = default!;
    public int IdDistrito { get; set; }
    public int IdDepartamento { get; set; }
    public int IdProvincia { get; set; }

    public int IdTipoUtilizacion { get; set; }
    public DateOnly FechaEspecificacion { get; set; }

    public string? Observaciones { get; set; }

    /// <summary>IFormFile para imagen del vacuno (JPG, PNG, JPEG)</summary>
    public IFormFile? Foto { get; set; }
}