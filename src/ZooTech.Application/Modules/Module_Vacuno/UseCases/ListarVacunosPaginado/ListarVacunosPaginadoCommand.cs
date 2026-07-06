using ZooTech.Domain.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;

public class ListarVacunosPaginadoCommand
{
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public EstadoAnimal? Estado { get; set; }
    public string? Q { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}
