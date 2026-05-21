namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;

public interface IListarCelosUseCase
{
    Task<List<CeloListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default);
}
