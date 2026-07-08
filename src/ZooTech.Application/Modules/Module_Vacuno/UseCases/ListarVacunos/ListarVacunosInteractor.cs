
using ZooTech.Application.Common.Configuration;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed class ListarVacunosInteractor : IListarVacunosInputPort
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IVacunoRepository _vacunoRepository;
    private readonly IVacunosConfiguration _settings;

    public ListarVacunosInteractor(IVacunoRepository vacunoRepository, IVacunosConfiguration settings)
    {
        _vacunoRepository = vacunoRepository;
        _settings = settings;
    }

    public async Task<ListarVacunosOutput> HandleAsync(ListarVacunosCommand command, CancellationToken cancellationToken = default)
    {
        var fechaDesde = command.FechaDesde;
        if (!fechaDesde.HasValue && !command.FechaHasta.HasValue)
        {
            fechaDesde = DateTime.UtcNow.AddDays(-_settings.DefaultFilterDays);
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
