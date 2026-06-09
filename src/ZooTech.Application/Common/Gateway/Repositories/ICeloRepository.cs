using ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;

namespace ZooTech.Application.Common.Gateway.Repositories;

public interface ICeloRepository
{
    Task<List<CeloListItemDto>> ListarCelosAsync(
        CancellationToken cancellationToken = default);

    Task<bool> EditarCeloAsync(
        long id,
        string? observaciones,
        List<string> caracteristicaCodes,
        CancellationToken cancellationToken = default);

    Task<bool> EliminarCeloAsync(
    long id,
    string motivoEliminacion,
    CancellationToken cancellationToken = default);
}