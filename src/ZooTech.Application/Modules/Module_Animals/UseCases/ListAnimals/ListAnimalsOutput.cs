using ZooTech.Application.Common.Models;
using ZooTech.Domain.Enums;

namespace ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;

public class ListAnimalsOutput
{
    public PagedResult<AnimalResumen> PagedData { get; set; } = new();
}

public class AnimalResumen
{
    public long Id { get; set; }
    public string Codigo { get; set; } = default!;
    public DateTime FechaRegistro { get; set; }
    public string Nombre { get; set; } = default!;
    public string Raza { get; set; } = default!;
    public string Procedencia { get; set; } = default!;
    public EstadoAnimal Estado { get; set; }
}
