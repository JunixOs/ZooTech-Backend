
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed class ListarVacunosInteractor : IListarVacunosInputPort
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IVacunoRepository _vacunoRepository;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;
    private readonly IAppCacheService _cache;

    public ListarVacunosInteractor(
        IVacunoRepository vacunoRepository, 
        ITenantConfigurationProvider tenantConfigurationProvider,
        IAppCacheService cache
    )
    {
        _vacunoRepository = vacunoRepository;
        _tenantConfigurationProvider = tenantConfigurationProvider;
        _cache = cache;
    }

    public async Task<ListarVacunosOutput> HandleAsync(ListarVacunosCommand command, CancellationToken cancellationToken = default)
    {
        var fechaDesde = command.FechaDesde;
        if (!fechaDesde.HasValue && !command.FechaHasta.HasValue)
        {
            var defaultFilterDays = await _tenantConfigurationProvider.GetSettingAsync(
                Settings.Vacunos.VacunosDefaultFilterDays
            );

            fechaDesde = DateTime.UtcNow.AddDays(-defaultFilterDays);
        }

        var page = command.Page <= 0 ? DefaultPage : command.Page;
        var pageSize = command.Limit <= 0 ? DefaultPageSize : Math.Min(command.Limit, MaxPageSize);

        var cacheKey = VacunoCacheKeys.Listar(
            command.Query,
            fechaDesde,
            command.FechaHasta,
            command.Estado,
            page,
            pageSize);

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var (items, totalCount) = await _vacunoRepository.GetPagedAsync(
                    command.Query,
                    fechaDesde,
                    command.FechaHasta,
                    command.Estado,
                    page,
                    pageSize,
                    cancellationToken);

                return new ListarVacunosOutput(items, totalCount);
            });
    }
}
