using ZooTech.Application.Common.Gateway.Repositories;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;

public class ListarCelosUseCase : IListarCelosUseCase
{
    private readonly ICeloRepository _celoRepository;

    public ListarCelosUseCase(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public Task<List<CeloListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return _celoRepository.ListarCelosAsync(cancellationToken);
    }
}
