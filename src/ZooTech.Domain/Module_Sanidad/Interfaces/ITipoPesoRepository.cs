using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ITipoPesoRepository
{
    Task<IEnumerable<TipoPeso>> GetAllAsync(CancellationToken cancellationToken = default);
}
