using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;

public sealed class ListOrdeniosInteractor : IListOrdeniosInputPort
{
    private readonly IOrdenioRepository _repository;

    public ListOrdeniosInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ListOrdeniosOutput> HandleAsync(ListOrdeniosQuery query, CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 100);
        var normalized = query with { Page = page, PageSize = pageSize };
       
        var (items, totalCount) = await _repository.ListAsync(normalized, cancellationToken);
        return new ListOrdeniosOutput(items, totalCount);
    }
}
