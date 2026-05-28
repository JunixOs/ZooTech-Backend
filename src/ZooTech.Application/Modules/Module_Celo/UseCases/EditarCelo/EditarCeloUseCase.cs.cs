using ZooTech.Application.Common.Gateway.Repositories;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.EditarCelo;

public class EditarCeloUseCase
{
    private readonly ICeloRepository _celoRepository;

    public EditarCeloUseCase(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<bool> ExecuteAsync(
        long id,
        string? observaciones,
        List<string> caracteristicaCodes,
        CancellationToken cancellationToken = default)
    {
        return await _celoRepository.EditarCeloAsync(
            id,
            observaciones,
            caracteristicaCodes,
            cancellationToken);
    }
}