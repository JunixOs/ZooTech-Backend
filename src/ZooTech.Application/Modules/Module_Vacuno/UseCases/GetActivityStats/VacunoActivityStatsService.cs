using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

public interface IVacunoActivityStatsService
{
    Task<GetActivityStatsOutput> GetAsync(
        DateOnly? fechaInicio,
        DateOnly? fechaFin,
        CancellationToken cancellationToken = default);
}

public sealed class VacunoActivityStatsService : IVacunoActivityStatsService
{
    private const int DefaultRangeDays = 30;
    private readonly IVacunoActivityStatsReadRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public VacunoActivityStatsService(
        IVacunoActivityStatsReadRepository repository,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GetActivityStatsOutput> GetAsync(
        DateOnly? fechaInicio,
        DateOnly? fechaFin,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(_dateTimeProvider.ServerNow);
        var end = fechaFin ?? today;
        var start = fechaInicio ?? end.AddDays(-(DefaultRangeDays - 1));

        if (start > end)
        {
            throw new ValidationException(
                ["La fecha de inicio no puede ser posterior a la fecha de fin."],
                ScopeName.Application,
                ModuleName.Vacuno);
        }

        var aggregates = await _repository.GetActivityAggregatesAsync(
            start,
            end,
            cancellationToken);
        var points = new List<GetActivityPointOutput>();
        var currentActive = aggregates.InventarioInicial;

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            currentActive += aggregates.AltasPorDia.GetValueOrDefault(date, 0);
            currentActive -= aggregates.BajasPorDia.GetValueOrDefault(date, 0);
            points.Add(new GetActivityPointOutput(date.ToString("yyyy-MM-dd"), currentActive));
        }

        return new GetActivityStatsOutput(
            start.ToString("yyyy-MM-dd"),
            end.ToString("yyyy-MM-dd"),
            points,
            points.Count == 0 ? 0 : points.Max(point => point.Cantidad),
            points.Count == 0 ? 0 : points.Min(point => point.Cantidad));
    }
}
