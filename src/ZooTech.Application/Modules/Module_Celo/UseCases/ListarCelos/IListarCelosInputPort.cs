namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;

public interface IListarCelosInputPort
{
    Task<ListarCelosOutput> HandleAsync(CancellationToken cancellationToken = default);
}
