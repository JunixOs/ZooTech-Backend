using ZooTech.Application.Common.Gateway.Repositories;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.EliminarCelo;

public class EliminarCeloUseCase
{
    private readonly ICeloRepository _celoRepository;

    public EliminarCeloUseCase(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<bool> ExecuteAsync(
        long id,
        string motivoEliminacion,
        CancellationToken cancellationToken = default)
    {
        return await _celoRepository.EliminarCeloAsync(
            id,
            motivoEliminacion,
            cancellationToken);
    }
}