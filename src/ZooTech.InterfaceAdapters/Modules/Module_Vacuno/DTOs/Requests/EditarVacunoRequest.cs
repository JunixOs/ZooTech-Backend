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
    public string Departamento { get; set; } = default!;
    public string Provincia { get; set; } = default!;
    public string AptoPara { get; set; } = default!;
    public DateOnly FechaEspecificacion { get; set; }
    public string? Observaciones { get; set; }
    public IFormFile? Foto { get; set; }
}
