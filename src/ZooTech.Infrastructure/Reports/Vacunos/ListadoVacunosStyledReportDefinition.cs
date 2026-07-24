using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

internal static class ListadoVacunosStyledReportDefinition
{
    public static StyledTabularReport<VacunoListadoReporteItem> Create(
        ListadoVacunosReportModel model,
        string extension)
        => new(
            new StyledReportMetadata(
                "Reporte de listado de vacunos",
                "Listado de vacunos",
                $"listado_vacunos{extension}",
                IsLandscape: true),
            VacunoReportTheme.Styled,
            model.Items,
            [
                new("Código", item => item.Codigo, 1.1f),
                new("Nombre", item => item.Nombre, 1.5f),
                new("Fecha nacimiento", item => item.FechaNacimiento, 1.1f),
                new("Fecha registro", item => item.FechaRegistro, 1.1f),
                new("Raza", item => item.Raza ?? "-", 1.2f),
                new("Sexo", item => item.Sexo ?? "-", 1),
                new("Granja", item => item.Granja ?? "-", 1.1f),
                new("Procedencia", item => item.Procedencia ?? "-", 2),
                new("Estado", item => item.Estado, 1),
                new("Estado registro", item => item.EstadoRegistro, 1.1f)
            ],
            [
                new StyledReportSummary("Filtros", BuildFilterText(model.Filtros))
            ]);

    private static string BuildFilterText(ReporteVacunoListadoFiltros filtros)
    {
        var filters = new[]
        {
            filtros.FechaDesde.HasValue ? $"Desde: {filtros.FechaDesde:yyyy-MM-dd}" : null,
            filtros.FechaHasta.HasValue ? $"Hasta: {filtros.FechaHasta:yyyy-MM-dd}" : null,
            !string.IsNullOrWhiteSpace(filtros.Q) ? $"Búsqueda: {filtros.Q}" : null,
            !string.IsNullOrWhiteSpace(filtros.Estado) ? $"Estado: {filtros.Estado}" : null,
            !string.IsNullOrWhiteSpace(filtros.Raza) ? $"Raza: {filtros.Raza}" : null
        };

        var value = string.Join(" | ", filters.Where(filter => filter is not null));
        return string.IsNullOrWhiteSpace(value) ? "Sin filtros" : value;
    }
}
