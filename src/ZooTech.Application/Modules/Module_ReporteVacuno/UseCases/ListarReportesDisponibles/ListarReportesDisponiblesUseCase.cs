using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ReporteVacuno.Common;

namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;

public sealed class ListarReportesDisponiblesUseCase : IListarReportesDisponiblesUseCase
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ListarReportesDisponiblesUseCase> _logger;

    public ListarReportesDisponiblesUseCase(
        IDateTimeProvider dateTimeProvider,
        ILogger<ListarReportesDisponiblesUseCase> logger)
    {
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public ReportesDisponiblesResponse Handle(ListarReportesDisponiblesQuery query)
    {
        var rango = ReporteVacunoDateRangeResolver.Resolve(
            query.FechaDesde,
            query.FechaHasta,
            _dateTimeProvider.Today);

        _logger.LogInformation(
            "[ReporteVacuno] rango aplicado: [{FechaDesde} - {FechaHasta}]",
            rango.FechaDesde,
            rango.FechaHasta);

        return new ReportesDisponiblesResponse(
            [
                new ReporteDisponibleItem(
                    "listado_vacunos",
                    "Reporte listado de vacunos",
                    "Lista de vacunos filtrados por fechas, estado y procedencia."),
                new ReporteDisponibleItem(
                    "registro_vacuno",
                    "Reporte registro por vacuno",
                    "Detalle completo del vacuno y su historial relacionado."),
                new ReporteDisponibleItem(
                    "grafico_genealogico",
                    "Grafico genealogico",
                    "Arbol genealogico del vacuno hasta 4 niveles."),
                new ReporteDisponibleItem(
                    "grafico_actividad",
                    "Grafico de actividad",
                    "Cantidad de vacunos en actividad por periodo.")
            ],
            new ReportesDisponiblesFiltros(
                rango.FechaDesde,
                rango.FechaHasta,
                Normalize(query.Q)));
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }
}
