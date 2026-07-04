using ZooTech.Domain.Module_Celo.Entities;

namespace ZooTech.Domain.Module_Celo.Interfaces;

public interface ICeloRepository
{
    Task<List<Celo>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Dictionary<long, int>> GetVecesEnCeloCountsAsync(
        CancellationToken cancellationToken = default);

    Task<Celo?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Celo> AddAsync(
        Celo celo,
        CancellationToken cancellationToken = default);

    Task<Celo> UpdateAsync(
        Celo celo,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsVacunoAsync(
        long vacunoId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<List<Celo>> GetByDateRangeAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default);
}