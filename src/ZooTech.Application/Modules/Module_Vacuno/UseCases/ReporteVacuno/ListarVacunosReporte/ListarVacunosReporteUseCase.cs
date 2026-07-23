using System.Globalization;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public sealed class ListarVacunosReporteUseCase : IListarVacunosReporteUseCase
{
    private const int DefaultPage = 1;
    private const int DefaultLimit = 10;
    private const int MaxLimit = 100;
    private const int ExportPageSize = 100;
    private readonly IListadoVacunosReporteReadRepository _repository;
    private readonly IReportStrategyResolver<ListadoVacunosReportModel> _strategyResolver;
    private readonly IVacunoReportFormatPolicy _formatPolicy;
    private readonly IReportFileStorage _fileStorage;

    public ListarVacunosReporteUseCase(
        IListadoVacunosReporteReadRepository repository,
        IReportStrategyResolver<ListadoVacunosReportModel> strategyResolver,
        IVacunoReportFormatPolicy formatPolicy,
        IReportFileStorage fileStorage)
    {
        _repository = repository;
        _strategyResolver = strategyResolver;
        _formatPolicy = formatPolicy;
        _fileStorage = fileStorage;
    }

    public async Task<ListarVacunosReporteResponse> HandleAsync(
        ListarVacunosReporteQuery query,
        CancellationToken cancellationToken = default)
    {
        var fechaDesde = ParseDate(query.FechaDesde);
        var fechaHasta = ParseDate(query.FechaHasta);
        var fechaRegistro = ParseDate(query.FechaRegistro);
        var formato = Normalize(query.Formato) ?? "json";
        var page = ParsePositiveInt(query.Page, DefaultPage);
        var limit = Math.Min(ParsePositiveInt(query.PageSize ?? query.Limit, DefaultLimit), MaxLimit);
        var search = Normalize(query.Search) ?? Normalize(query.Q);

        var filtros = new ReporteVacunoListadoFiltros(
            fechaDesde,
            fechaHasta,
            search,
            Normalize(query.Codigo),
            fechaRegistro,
            Normalize(query.Nombre),
            Normalize(query.Raza),
            Normalize(query.Procedencia),
            NormalizeEstado(query.Estado),
            NormalizeEstadoRegistro(query.EstadoRegistro),
            Normalize(query.AptoPara),
            formato);

        var result = await _repository.ListarAsync(
            BuildReadQuery(filtros, page, limit),
            cancellationToken);
        var items = MapItems(result.Items);

        if (formato is "excel" or "pdf")
        {
            var reportFormat = await _formatPolicy.EnsureAllowedAsync(formato);
            var exportItems = await LoadAllAsync(filtros, cancellationToken);
            var document = await _strategyResolver
                .Resolve(reportFormat)
                .GenerateAsync(
                    new ListadoVacunosReportModel(exportItems.Items, filtros),
                    cancellationToken);
            var storedFile = await _fileStorage.SaveAsync(
                "listado_vacunos",
                document.Extension,
                document.ContentType,
                document.Content,
                cancellationToken);

            return new ListarVacunosReporteResponse(
                items,
                new ReporteVacunoListadoResumen(result.Total),
                filtros,
                storedFile.DownloadUrl,
                result.Total,
                page,
                limit);
        }

        return new ListarVacunosReporteResponse(
            items,
            new ReporteVacunoListadoResumen(result.Total),
            filtros,
            null,
            result.Total,
            page,
            limit);
    }

    private async Task<(IReadOnlyCollection<VacunoListadoReporteItem> Items, int Total)> LoadAllAsync(
        ReporteVacunoListadoFiltros filtros,
        CancellationToken cancellationToken)
    {
        var items = new List<VacunoListadoReporteItem>();
        var page = 1;
        var total = 0;

        do
        {
            var result = await _repository.ListarAsync(
                BuildReadQuery(filtros, page, ExportPageSize),
                cancellationToken);

            total = result.Total;
            var pageItems = MapItems(result.Items);
            items.AddRange(pageItems);
            page++;

            if (pageItems.Count == 0)
            {
                break;
            }
        }
        while (items.Count < total);

        return (items, total);
    }

    private static ListadoVacunosReporteReadQuery BuildReadQuery(
        ReporteVacunoListadoFiltros filtros,
        int page,
        int limit)
        => new(
            filtros.FechaDesde,
            filtros.FechaHasta,
            filtros.Q,
            filtros.Codigo,
            filtros.FechaRegistro,
            filtros.Nombre,
            filtros.Raza,
            filtros.Procedencia,
            filtros.Estado,
            filtros.EstadoRegistro,
            filtros.AptoPara,
            page,
            limit);

    private static IReadOnlyCollection<VacunoListadoReporteItem> MapItems(
        IReadOnlyCollection<VacunoListadoReporteReadItem> items)
        => items
            .Select(item => new VacunoListadoReporteItem(
                item.Id,
                item.Codigo,
                item.FechaNacimiento,
                item.FechaRegistro,
                item.Nombre,
                item.TipoAdquisicion,
                item.Raza,
                item.Color,
                item.Sexo,
                item.Granja,
                item.Procedencia,
                item.Estado,
                item.EstadoRegistro))
            .ToList();

    private static DateOnly? ParseDate(string? value)
        => DateOnly.TryParseExact(
            value,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date)
                ? date
                : null;

    private static int ParsePositiveInt(string? value, int fallback)
        => int.TryParse(value, out var number) && number > 0 ? number : fallback;

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string? NormalizeEstado(string? value)
    {
        var normalized = Normalize(value);
        var lower = normalized?.ToLowerInvariant();
        if (lower is "vivo" or "muerto")
        {
            return lower;
        }

        var upper = normalized?.ToUpperInvariant();
        return upper is "SANO" or "ENFERMO" or "CUARENTENA" or "MUERTO" ? upper : null;
    }

    private static string? NormalizeEstadoRegistro(string? value)
    {
        var normalized = Normalize(value)?.ToLowerInvariant();
        return normalized is "activo" or "eliminado" ? normalized : null;
    }
}
