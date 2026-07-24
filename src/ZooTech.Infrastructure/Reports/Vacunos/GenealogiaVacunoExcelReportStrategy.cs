using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class GenealogiaVacunoExcelReportStrategy
    : IReportStrategy<GenealogiaVacunoReportModel>
{
    private readonly IStyledExcelReportRenderer _renderer;

    public GenealogiaVacunoExcelReportStrategy(IStyledExcelReportRenderer renderer)
    {
        _renderer = renderer;
    }

    public ReportFileFormat Format => ReportFileFormat.Excel;

    public Task<GeneratedReportDocument> GenerateAsync(
        GenealogiaVacunoReportModel model,
        CancellationToken cancellationToken = default)
        => _renderer.RenderAsync(
            GenealogiaVacunoStyledReportDefinition.Create(model, ".xlsx"),
            cancellationToken);
}
