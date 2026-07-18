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
        ListarFecundacionCommand command, CancellationToken cancellationToken = default)
    {
        var page = command.Page <= 0 ? DefaultPage : command.Page;
        var pageSize = command.Limit <= 0 ? DefaultPageSize : Math.Min(command.Limit, MaxPageSize);
        
        var cacheKey = FecundacionCacheKeys.Listar(
            command.Query,
            command.FechaDesde,
            command.FechaHasta,
            command.Resultado,
            page,
            pageSize);

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var (items, totalCount) = await _fecundacionRepository.GetPagedAsync(
                    command.Query,
                    command.FechaDesde,
                    command.FechaHasta,
                    command.Resultado,
                    page,
                    pageSize,
                    cancellationToken);

                return new ListarFecundacionOutput(items, totalCount);
            });
    }
}
