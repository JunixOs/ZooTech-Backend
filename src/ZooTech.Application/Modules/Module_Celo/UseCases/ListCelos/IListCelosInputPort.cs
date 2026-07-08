namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;

public interface IListCelosInputPort
{
    Task<ListCelosOutput> HandleAsync(
        ListCelosCommand cmd,
        CancellationToken cancellationToken = default);
}
