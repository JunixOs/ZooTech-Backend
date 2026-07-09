using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

public sealed class GenerateOrdeniosPdfInteractor : IGetOrdeniosPdfInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IPdfGeneratorService _pdfGeneratorService;
    private readonly IOrdeniosComparationPdfGeneratorService _comparationPdfGeneratorService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GenerateOrdeniosPdfInteractor(
        IOrdenioRepository repository,
        IPdfGeneratorService pdfGeneratorService,
        IOrdeniosComparationPdfGeneratorService comparationPdfGeneratorService,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _pdfGeneratorService = pdfGeneratorService;
        _comparationPdfGeneratorService = comparationPdfGeneratorService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GenerateOrdeniosPdfOutput> Handle(
        GenerateOrdeniosComparationPdfQuery query,
        CancellationToken cancellationToken)
    {
        var entities = await _repository.ListReportAsync(
            query.VacunoId,
            query.EstadoOrdenioCode,
            query.FechaDesde,
            query.FechaHasta,
            cancellationToken);

        var document = new GenerateOrdeniosPdfDocument(
            entities.Select(OrdenioMapper.ToOutput).ToList(),
            query.VacunoId,
            query.EstadoOrdenioCode,
            query.FechaDesde,
            query.FechaHasta,
            _dateTimeProvider.ServerNow);

        var pdfContent = query.Comparativo
            ? _comparationPdfGeneratorService.GenerateOrdeniosReport(document)
            : _pdfGeneratorService.GenerateOrdeniosReport(document);

        var fileNamePrefix = query.Comparativo
            ? "reporte-comparativo-ordenios"
            : "reporte-ordenios";

        return new GenerateOrdeniosPdfOutput(
            pdfContent,
            "application/pdf",
            $"{fileNamePrefix}-{document.GeneratedAtUtc:yyyyMMddHHmmss}.pdf");
    }
}
