using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class ListadoVacunosPdfReportService
    : IReportStrategy<ListadoVacunosReportModel>
{
    private readonly IStyledPdfReportRenderer _renderer;

    public ListadoVacunosPdfReportService(IStyledPdfReportRenderer renderer)
    {
        _renderer = renderer;
    }

    public ReportFileFormat Format => ReportFileFormat.Pdf;

    public Task<GeneratedReportDocument> GenerateAsync(
        ListadoVacunosReportModel model,
        CancellationToken cancellationToken = default)
        => _renderer.RenderAsync(
            ListadoVacunosStyledReportDefinition.Create(model, ".pdf"),
            cancellationToken);
}
