using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarReportesDisponibles;

public sealed class ListarReportesDisponiblesUseCase : IListarReportesDisponiblesUseCase
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public ListarReportesDisponiblesUseCase(
        IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ReportesDisponiblesResponse> HandleAsync(ListarReportesDisponiblesQuery query, CancellationToken cancellationToken = default)
    {
        var dateFormat = "yyyy-MM-dd";

        var rango = ReporteVacunoDateRangeResolver.Resolve(
            query.FechaDesde,
            query.FechaHasta,
            dateFormat);

        var catalog = new ReporteDisponibleItem[] 
        {
            new("listado-vacunos", "Listado General de Vacunos", "Reporte de vacunos en el sistema")
        };

        return new ReportesDisponiblesResponse(
            catalog,
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

