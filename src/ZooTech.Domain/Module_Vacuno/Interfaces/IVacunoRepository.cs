using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Domain.Module_Vacuno.Interfaces;

public interface IVacunoRepository
{
    Task<List<Vacuno>> ListAllAsync(CancellationToken cancellationToken = default);

    Task<Vacuno?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
}
