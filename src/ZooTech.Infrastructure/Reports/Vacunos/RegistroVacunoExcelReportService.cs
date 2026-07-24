using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class RegistroVacunoExcelReportService
    : IReportStrategy<RegistroVacunoDetalle>
{
    private readonly IStyledExcelReportRenderer _renderer;

    public RegistroVacunoExcelReportService(IStyledExcelReportRenderer renderer)
    {
        _renderer = renderer;
    }

    public ReportFileFormat Format => ReportFileFormat.Excel;

    public Task<GeneratedReportDocument> GenerateAsync(
        RegistroVacunoDetalle model,
        CancellationToken cancellationToken = default)
        => _renderer.RenderDetailAsync(
            RegistroVacunoStyledReportDefinition.Create(model, ".xlsx"),
            cancellationToken);
}
