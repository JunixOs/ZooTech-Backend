using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Vacunos.UseCases.ListarVacunos;

public class ListarVacunosInteractor : IListarVacunosInputPort
{
    private readonly IVacunoRepository _repository;
    private readonly IListarVacunosOutputPort _output;
    private readonly IFeatureService _features;
    private readonly ITenantContext _tenant;
    private readonly IDateTimeProvider _time;

    public ListarVacunosInteractor(
        IVacunoRepository repository,
        IListarVacunosOutputPort output,
        IFeatureService features,
        ITenantContext tenant,
        IDateTimeProvider time)
    {
        _repository = repository;
        _output = output;
        _features = features;
        _tenant = tenant;
        _time = time;
    }

    public async Task Handle(ListarVacunosCommand cmd)
    {
        // 1. Validar Feature Flag
        if (!await _features.IsEnabledAsync("module.vacunos"))
        {
            await _output.Error("FEATURE_DISABLED", "El módulo de vacunos no está habilitado para este tenant.");
            return;
        }

        // 2. Filtro de 30 días por defecto si no hay fechas
        var fechaDesde = cmd.FechaDesde;
        var fechaHasta = cmd.FechaHasta;

        if (!fechaDesde.HasValue && !fechaHasta.HasValue)
        {
            fechaHasta = _time.UtcNow.Date;
            fechaDesde = fechaHasta.Value.AddDays(-30);
        }

        // 3. Paginación
        int skip = (cmd.Page - 1) * cmd.Limit;
        int take = cmd.Limit;

        // 4. Obtener datos proyectados desde el repositorio
        var (data, total) = await _repository.GetPagedAsync(
            fechaDesde,
            fechaHasta,
            cmd.Estado,
            cmd.Q,
            skip,
            take);

        // 5. Retornar por el Output Port
        await _output.Ok(new ListarVacunosOutput
        {
            PagedData = new PagedResult<VacunoResumen>
            {
                Data = data,
                Total = total,
                Page = cmd.Page,
                PageSize = cmd.Limit
            }
        });
    }
}
