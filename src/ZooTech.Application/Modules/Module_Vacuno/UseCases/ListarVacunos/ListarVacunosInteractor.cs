
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed class ListarVacunosInteractor : IListarVacunosInputPort
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IVacunoRepository _vacunoRepository;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

    public ListarVacunosInteractor(
        IVacunoRepository vacunoRepository, 
        ITenantConfigurationProvider tenantConfigurationProvider
    )
    {
        _vacunoRepository = vacunoRepository;
        _tenantConfigurationProvider = tenantConfigurationProvider;
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

        var (items, totalCount) = await _vacunoRepository.GetPagedAsync(
            command.Query,
            fechaDesde,
            command.FechaHasta,
            command.Estado,
            page,
            pageSize,
            cancellationToken);

        return new ListarVacunosOutput(items, totalCount);
    }
}
