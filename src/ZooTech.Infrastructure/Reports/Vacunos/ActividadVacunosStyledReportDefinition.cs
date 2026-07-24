using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

internal static class ActividadVacunosStyledReportDefinition
{
    public static StyledTabularReport<GetActivityPointOutput> Create(
        ActividadVacunosReportModel model,
        string extension)
    {
        var stats = model.Stats;
        return new StyledTabularReport<GetActivityPointOutput>(
            new StyledReportMetadata(
                "Reporte de actividad de vacunos",
                "Actividad de vacunos",
                $"Actividad_Vacunos_{stats.FechaInicio}_{stats.FechaFin}{extension}"),
            VacunoReportTheme.Styled,
            stats.Points,
            [
                new("Fecha", point => point.Fecha),
                new("Vacunos activos", point => point.Cantidad)
            ],
            [
                new StyledReportSummary("Rango", $"{stats.FechaInicio} - {stats.FechaFin}"),
                new StyledReportSummary("Máximo de activos", stats.Mayor.ToString()),
                new StyledReportSummary("Mínimo de activos", stats.Menor.ToString())
            ]);
    }
}
