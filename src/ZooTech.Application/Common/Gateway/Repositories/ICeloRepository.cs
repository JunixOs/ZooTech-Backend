using ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;

namespace ZooTech.Application.Common.Gateway.Repositories;

public interface ICeloRepository
{
    Task<List<CeloListItemDto>> ListarCelosAsync(CancellationToken cancellationToken = default);
}
