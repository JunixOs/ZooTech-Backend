using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

public sealed class GenerateOrdeniosPdfInteractor : IGetOrdeniosPdfInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IPdfGeneratorService _pdfGeneratorService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GenerateOrdeniosPdfInteractor(
        IOrdenioRepository repository,
        IPdfGeneratorService pdfGeneratorService,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _pdfGeneratorService = pdfGeneratorService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GenerateOrdeniosPdfOutput> HandleAsync(
        GenerateOrdeniosPdfQuery query,
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

        return new GenerateOrdeniosPdfOutput(
            _pdfGeneratorService.GenerateOrdeniosReport(document),
            "application/pdf",
            $"reporte-ordenios-{document.GeneratedAtUtc:yyyyMMddHHmmss}.pdf");
    }
}
