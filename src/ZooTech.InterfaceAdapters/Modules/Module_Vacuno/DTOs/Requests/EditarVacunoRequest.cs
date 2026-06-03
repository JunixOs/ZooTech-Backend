using Microsoft.AspNetCore.Http;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

public sealed class EditarVacunoRequest
{
    public string Nombre { get; set; } = default!;
    public string AdquisicionPor { get; set; } = "monta";
    public decimal? PrecioCompra { get; set; }
    public string Raza { get; set; } = default!;
    public string Color { get; set; } = default!;
    public string Sexo { get; set; } = default!;
    public string Granja { get; set; } = default!;
    public string Distrito { get; set; } = default!;
    public string? CodigoDistrito { get; set; }
    public string Departamento { get; set; } = default!;
    public string? CodigoDepartamento { get; set; }
    public string Provincia { get; set; } = default!;
    public string? CodigoProvincia { get; set; }
    public string AptoPara { get; set; } = default!;
    public DateOnly FechaEspecificacion { get; set; }
    public string? Observaciones { get; set; }
    public IFormFile? Foto { get; set; }
}
