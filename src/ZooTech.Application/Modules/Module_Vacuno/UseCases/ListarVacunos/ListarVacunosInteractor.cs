using System.Globalization;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed class ListarVacunosInteractor : IListarVacunosInputPort
{
    private readonly IVacunoRepository _repository;
    private readonly IListadoVacunosReportFileService _reportFileService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ListarVacunosInteractor(
        IVacunoRepository repository,
        IListadoVacunosReportFileService reportFileService,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _reportFileService = reportFileService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ListarVacunosOutput> HandleAsync(
        ListarVacunosCommand command,
        CancellationToken cancellationToken = default)
    {
        var defaultDays = 30;
        var dateFormat = "yyyy-MM-dd";
        var formato = Normalize(command.Formato) ?? "json";

        var rango = ReporteVacunoDateRangeResolver.Resolve(
            command.FechaDesde,
            command.FechaHasta,
            DateOnly.FromDateTime(_dateTimeProvider.ServerNow),
            defaultDays,
            dateFormat);

        var page = ParsePositiveIntOrDefault(command.Page, 1);
        var limit = ParsePositiveIntOrDefault(command.Limit, 20);
        limit = Math.Min(limit, 100);

        var criteria = new ListarVacunosCriteriaDomain(
            rango.FechaDesde,
            rango.FechaHasta,
            Normalize(command.Q),
            Normalize(command.Raza),
            Normalize(command.Procedencia),
            Normalize(command.Estado),
            Normalize(command.AptoPara),
            page,
            limit);

        var pageResult = await _repository.ListarAvanzadoAsync(criteria, cancellationToken);
        var mappedItems = pageResult.Items.Select(x => new VacunoListadoItem(x.Id, x.Codigo, x.FechaRegistro, x.Nombre, x.FechaNacimiento, x.RazaCode, x.SexoCode, x.Raza, x.Procedencia, x.Estado)).ToList();

        var resumen = new ListarVacunosResumen(pageResult.TotalRegistros);
        var filtros = new ListarVacunosFiltros(
            rango.FechaDesde,
            rango.FechaHasta,
            criteria.Q,
            criteria.Raza,
            criteria.Procedencia,
            criteria.Estado,
            criteria.AptoPara,
            formato);

        var downloadUrl = formato switch
        {
            "excel" => (await _reportFileService.GenerateExcelAsync(mappedItems, resumen, filtros, cancellationToken)).DownloadUrl,
            "pdf" => (await _reportFileService.GeneratePdfAsync(mappedItems, resumen, filtros, cancellationToken)).DownloadUrl,
            _ => null
        };

        return new ListarVacunosOutput(
            mappedItems,
            resumen,
            filtros,
            downloadUrl);
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized.ToLowerInvariant();
    }

    private static int ParsePositiveIntOrDefault(string? value, int defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (!int.TryParse(value.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var parsed))
        {
            return defaultValue;
        }

        return parsed < 1 ? defaultValue : parsed;
    }
}
