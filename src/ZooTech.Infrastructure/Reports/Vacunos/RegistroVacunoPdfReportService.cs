using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class RegistroVacunoPdfReportService
    : IReportStrategy<RegistroVacunoDetalle>
{
    private readonly IStyledPdfReportRenderer _renderer;
    private readonly IVacunoReportPhotoLoader _photoLoader;

    public RegistroVacunoPdfReportService(
        IStyledPdfReportRenderer renderer,
        IVacunoReportPhotoLoader photoLoader)
    {
        _renderer = renderer;
        _photoLoader = photoLoader;
    }

    public ReportFileFormat Format => ReportFileFormat.Pdf;

    public async Task<GeneratedReportDocument> GenerateAsync(
        RegistroVacunoDetalle model,
        CancellationToken cancellationToken = default)
    {
        var photo = await _photoLoader.LoadAsync(model, cancellationToken);
        return await _renderer.RenderDetailAsync(
            RegistroVacunoStyledReportDefinition.Create(model, ".pdf", photo),
            cancellationToken);
    }
}
