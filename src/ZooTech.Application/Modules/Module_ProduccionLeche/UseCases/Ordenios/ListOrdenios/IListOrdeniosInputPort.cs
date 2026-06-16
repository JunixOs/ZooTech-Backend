namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;

public interface IListOrdeniosInputPort
{
    Task<ListOrdeniosOutput> HandleAsync(ListOrdeniosQuery query, CancellationToken cancellationToken);
}
