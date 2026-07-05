using ZooTech.Domain.Module_Celo.Entities;

namespace ZooTech.Domain.Module_Celo.Interfaces;

public interface ICeloRepository
{
    Task<List<CeloListItem>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<List<CeloReporteItem>> GetAllForReporteAsync(
        CancellationToken cancellationToken = default);

    Task<Dictionary<long, int>> GetVecesEnCeloCountsAsync(
        CancellationToken cancellationToken = default);

    Task<Dictionary<long, int>> GetCriasCountsAsync(
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
}
