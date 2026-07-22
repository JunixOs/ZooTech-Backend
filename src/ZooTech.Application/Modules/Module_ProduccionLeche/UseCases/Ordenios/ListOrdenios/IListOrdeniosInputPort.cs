namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;

public interface IListOrdeniosInputPort
{
    Task<ListOrdeniosOutput> Handle(ListOrdeniosQuery query, CancellationToken cancellationToken);
}
