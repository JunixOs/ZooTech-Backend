using ZooTech.Application.Common.Models;
using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;

namespace ZooTech.Application.Common.Gateway.Repositories;

public interface IVacunoRepository
{
    Task<(List<Animal> Data, int Total)> GetPagedAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        EstadoAnimal? estado,
        string? q,
        int skip,
        int take);
}
