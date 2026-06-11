using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ISanidadVacunoRepository
{
    Task<IEnumerable<VacunoOption>> GetAllAsync(CancellationToken cancellationToken = default);
}
