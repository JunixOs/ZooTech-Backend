namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;

public interface IListCelosInputPort
{
    Task<ListCelosOutput> HandleAsync(
        string? search,
        int page,
        int pageSize,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        CancellationToken cancellationToken = default);
}
