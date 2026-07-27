
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed class ListarVacunosInteractor : IListarVacunosInputPort
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IVacunoRepository _vacunoRepository;
    private readonly IAppCacheService _cache;

    public ListarVacunosInteractor(
        IVacunoRepository vacunoRepository,
        IAppCacheService cache
    )
    {
        _vacunoRepository = vacunoRepository;
        _cache = cache;
    }

    public async Task<ListarVacunosOutput> HandleAsync(ListarVacunosQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page <= 0 ? DefaultPage : query.Page;
        var pageSize = query.Limit <= 0 ? DefaultPageSize : Math.Min(query.Limit, MaxPageSize);

        var cacheKey = VacunoCacheKeys.Listar(
            query.Query,
            query.FechaDesde,
            query.FechaHasta,
            query.Estado,
            page,
            pageSize);

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var (items, totalCount) = await _vacunoRepository.GetPagedAsync(
                    query.Query,
                    query.FechaDesde,
                    query.FechaHasta,
                    query.Estado,
                    page,
                    pageSize,
                    cancellationToken);

                return new ListarVacunosOutput(items, totalCount);
            });
    }
}
