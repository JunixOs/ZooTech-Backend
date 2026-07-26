using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public sealed class ListarFecundacionInteractor : IListarFecundacionInputPort
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IFecundacionRepository _fecundacionRepository;
    private readonly IAppCacheService _cache;

    public ListarFecundacionInteractor(
        IFecundacionRepository fecundacionRepository,
        IAppCacheService cache
    )
    {
        _fecundacionRepository = fecundacionRepository;
        _cache = cache;
    }

    public async Task<ListarFecundacionOutput> HandleAsync(
        ListarFecundacionQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page <= 0 ? DefaultPage : query.Page;
        var pageSize = query.Limit <= 0 ? DefaultPageSize : Math.Min(query.Limit, MaxPageSize);
        
        var cacheKey = FecundacionCacheKeys.Listar(
            query.Query,
            query.FechaDesde,
            query.FechaHasta,
            query.Resultado,
            page,
            pageSize);

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var (items, totalCount) = await _fecundacionRepository.GetPagedAsync(
                    query.Query,
                    query.FechaDesde,
                    query.FechaHasta,
                    query.Resultado,
                    page,
                    pageSize,
                    cancellationToken);

                return new ListarFecundacionOutput(items, totalCount);
            });
    }
}
