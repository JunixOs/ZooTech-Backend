using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

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

        var (entities, totalCount) = await _repository.ListAsync(
            query.VacunoId,
            query.EstadoOrdenioCode,
            query.FechaDesde,
            query.FechaHasta,
            page,
            pageSize,
            cancellationToken);

        var items = entities.Select(OrdenioMapper.ToOutput).ToList();
        return new ListOrdeniosOutput(items, totalCount);
    }
}
