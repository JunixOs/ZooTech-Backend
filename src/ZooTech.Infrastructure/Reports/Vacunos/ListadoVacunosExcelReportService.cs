using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class ListadoVacunosExcelReportService
    : IReportStrategy<ListadoVacunosReportModel>
{
    private readonly IStyledExcelReportRenderer _renderer;

    public ListadoVacunosExcelReportService(IStyledExcelReportRenderer renderer)
    {
        _renderer = renderer;
    }

    public ReportFileFormat Format => ReportFileFormat.Excel;

    public Task<GeneratedReportDocument> GenerateAsync(
        ListadoVacunosReportModel model,
        CancellationToken cancellationToken = default)
        => _renderer.RenderAsync(
            ListadoVacunosStyledReportDefinition.Create(model, ".xlsx"),
            cancellationToken);
}
