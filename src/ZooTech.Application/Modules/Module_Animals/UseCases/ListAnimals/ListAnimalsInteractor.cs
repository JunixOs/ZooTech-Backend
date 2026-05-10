using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;

public class ListAnimalsInteractor : IListAnimalsInputPort
{
    private readonly IAnimalRepository _repository;
    private readonly IListAnimalsOutputPort _output;
    private readonly IFeatureService _features;
    private readonly ITenantContext _tenant;
    private readonly IDateTimeProvider _time;

    public ListAnimalsInteractor(
        IAnimalRepository repository,
        IListAnimalsOutputPort output,
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

    public async Task Handle(ListAnimalsCommand cmd)
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

        // 4. Obtener datos (El repositorio usará el ITenantDbContextFactory internamente para el aislamiento del tenant)
        var (data, total) = await _repository.GetPagedAsync(
            fechaDesde,
            fechaHasta,
            cmd.Estado,
            cmd.Q,
            skip,
            take);

        // 5. Mapear a Resumen
        var resumenData = data.Select(a => new AnimalResumen
        {
            Id = a.Id.Value,
            Codigo = a.Codigo,
            FechaRegistro = a.FechaRegistro,
            Nombre = a.Nombre,
            Raza = a.Raza.Nombre,
            Procedencia = a.Procedencia.ToString(),
            Estado = a.Estado
        }).ToList();

        // 6. Retornar por el Output Port
        await _output.Ok(new ListAnimalsOutput
        {
            PagedData = new PagedResult<AnimalResumen>
            {
                Data = resumenData,
                Total = total,
                Page = cmd.Page,
                PageSize = cmd.Limit
            }
        });
    }
}
