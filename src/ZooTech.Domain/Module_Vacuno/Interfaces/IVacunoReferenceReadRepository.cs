using ZooTech.Domain.Module_Vacuno.ReadModels;

namespace ZooTech.Domain.Module_Vacuno.Interfaces;

public interface IVacunoReferenceReadRepository
{
    Task<VacunoCodeLookup?> GetActiveByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<VacunoCodeLookup?> GetActiveByCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsActiveGranjaAsync(
        long granjaId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsDistritoAsync(
        string codigoDistrito,
        CancellationToken cancellationToken = default);

    Task<long?> FindGranjaIdAsync(
        string nombre,
        string codigoDistrito,
        CancellationToken cancellationToken = default);
}
