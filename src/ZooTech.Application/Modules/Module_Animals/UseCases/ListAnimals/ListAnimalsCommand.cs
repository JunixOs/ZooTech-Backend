using ZooTech.Domain.Modules.Module_Animals.Enums;

namespace ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;

public class ListAnimalsCommand
{
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public EstadoAnimal? Estado { get; set; }
    public string? Q { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}
