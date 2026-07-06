using ZooTech.Application.Common.Models;
using ZooTech.Domain.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;

public class ListarVacunosPaginadoOutput
{
    public PagedResult<VacunoResumen> PagedData { get; set; } = new();
}

public class VacunoResumen
{
    public long Id { get; set; }
    public string Codigo { get; set; } = default!;
    public DateTime FechaRegistro { get; set; }
    public string Nombre { get; set; } = default!;
    public string Raza { get; set; } = default!;
    public string Procedencia { get; set; } = default!;
    public EstadoAnimal Estado { get; set; }
}
