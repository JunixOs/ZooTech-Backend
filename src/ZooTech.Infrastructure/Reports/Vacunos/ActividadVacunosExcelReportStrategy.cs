using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class ActividadVacunosExcelReportStrategy
    : IReportStrategy<ActividadVacunosReportModel>
{
    private readonly IStyledExcelReportRenderer _renderer;

    public ActividadVacunosExcelReportStrategy(IStyledExcelReportRenderer renderer)
    {
        _renderer = renderer;
    }

    public ReportFileFormat Format => ReportFileFormat.Excel;

    public Task<GeneratedReportDocument> GenerateAsync(
        ActividadVacunosReportModel model,
        CancellationToken cancellationToken = default)
        => _renderer.RenderAsync(
            ActividadVacunosStyledReportDefinition.Create(model, ".xlsx"),
            cancellationToken);
}
