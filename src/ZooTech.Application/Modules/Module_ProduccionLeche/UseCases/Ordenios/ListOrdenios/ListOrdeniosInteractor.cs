using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;
using ZooTech.Domain.Entities.Configuration;

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
        var page = query.Page <= 0 ?  ConfigSettings.Produccionleche.DefaultPage : query.Page;
        var pageSize = query.PageSize <= 0 ? ConfigSettings.Produccionleche.DefaultPageSize : Math.Min(query.PageSize, ConfigSettings.Produccionleche.MaxPageSize);
        var normalized = query with { Page = page, PageSize = pageSize };
       
        var (items, totalCount) = await _repository.ListAsync(normalized, cancellationToken);
        return new ListOrdeniosOutput(items, totalCount);
    }
}
