using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class ActividadVacunosPdfReportStrategy
    : IReportStrategy<ActividadVacunosReportModel>
{
    private readonly IStyledPdfReportRenderer _renderer;

    public ActividadVacunosPdfReportStrategy(IStyledPdfReportRenderer renderer)
    {
        _renderer = renderer;
    }

    public ReportFileFormat Format => ReportFileFormat.Pdf;

    public Task<GeneratedReportDocument> GenerateAsync(
        ActividadVacunosReportModel model,
        CancellationToken cancellationToken = default)
        => _renderer.RenderAsync(
            ActividadVacunosStyledReportDefinition.Create(model, ".pdf"),
            cancellationToken);
}
