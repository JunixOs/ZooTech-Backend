using ZooTech.Domain.Module_Vacuno.Criteria;
using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Domain.Module_Vacuno.Interfaces;

public interface IVacunoRepository
{
    Task<List<Vacuno>> ListAllAsync(CancellationToken cancellationToken = default);

    Task<List<(Vacuno Vacuno, string? Procedencia)>> ListAllForDisplayAsync(CancellationToken cancellationToken = default);

    Task<Vacuno?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<Vacuno> AddAsync(Vacuno vacuno, CancellationToken cancellationToken = default);

    Task<Vacuno> UpdateAsync(Vacuno vacuno, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Vacuno> Items, int TotalRegistros)> ListarAvanzadoAsync(
        ListarVacunosCriteriaDomain criteria,
        CancellationToken cancellationToken = default);
}
