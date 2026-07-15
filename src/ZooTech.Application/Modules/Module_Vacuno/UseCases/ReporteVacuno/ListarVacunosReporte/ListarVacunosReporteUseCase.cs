using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public sealed class ListarVacunosReporteUseCase : IListarVacunosReporteUseCase
{
    private const int DefaultPage = 1;
    private const int DefaultLimit = 10;
    private const int MaxLimit = 100;
    private readonly IListadoVacunosReporteReadRepository _repository;

    public ListarVacunosReporteUseCase(IListadoVacunosReporteReadRepository repository)
    {
        _repository = repository;
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

        var result = await _repository.ListarAsync(
            new ListadoVacunosReporteReadQuery(
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
                page,
                limit),
            cancellationToken);

        var items = result.Items
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

        return new ListarVacunosReporteResponse(
            items,
            new ReporteVacunoListadoResumen(result.Total),
            new ReporteVacunoListadoFiltros(
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
                formato),
            null,
            result.Total,
            page,
            limit);
    }

    private static DateOnly? ParseDate(string? value)
        => DateOnly.TryParse(value, out var date) ? date : null;

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
