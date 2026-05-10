using ZooTech.Application.Common.Models;
using ZooTech.Domain.Modules.Module_Animals.Entities;
using ZooTech.Domain.Modules.Module_Animals.Enums;

namespace ZooTech.Application.Common.Gateway.Repositories;

public interface IAnimalRepository
{
    Task<(List<Animal> Data, int Total)> GetPagedAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        EstadoAnimal? estado,
        string? q,
        int skip,
        int take);
}
