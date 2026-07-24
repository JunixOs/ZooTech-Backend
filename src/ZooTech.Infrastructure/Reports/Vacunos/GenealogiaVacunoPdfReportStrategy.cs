using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class GenealogiaVacunoPdfReportStrategy
    : IReportStrategy<GenealogiaVacunoReportModel>
{
    private readonly IStyledPdfReportRenderer _renderer;

    public GenealogiaVacunoPdfReportStrategy(IStyledPdfReportRenderer renderer)
    {
        _renderer = renderer;
    }

    public ReportFileFormat Format => ReportFileFormat.Pdf;

    public Task<GeneratedReportDocument> GenerateAsync(
        GenealogiaVacunoReportModel model,
        CancellationToken cancellationToken = default)
        => _renderer.RenderAsync(
            GenealogiaVacunoStyledReportDefinition.Create(model, ".pdf"),
            cancellationToken);
}
