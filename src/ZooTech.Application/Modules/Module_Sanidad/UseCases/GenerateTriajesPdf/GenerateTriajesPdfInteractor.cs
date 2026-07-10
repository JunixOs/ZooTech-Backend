using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

public sealed class GenerateTriajesPdfInteractor : IGenerateTriajesPdfInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly IPdfGeneratorService _pdfGeneratorService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GenerateTriajesPdfInteractor(
        ITriajeRepository repository,
        IPdfGeneratorService pdfGeneratorService,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _pdfGeneratorService = pdfGeneratorService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GenerateTriajesPdfOutput> HandleAsync(
        GenerateTriajesPdfQuery query,
        CancellationToken cancellationToken)
    {
        var (triajes, _) = await _repository.GetAllAsync(
            pagina: 1,
            tamano: int.MaxValue,
            query.Fecha,
            query.FechaDesde,
            query.FechaHasta,
            query.Codigo,
            query.Nombre,
            query.TipoPeso,
            query.PesoKg,
            query.VacunoId,
            cancellationToken: cancellationToken);

        var document = new GenerateTriajesPdfDocument(
            triajes.Select(TriajeMapper.ToOutput).ToList(),
            query.Fecha,
            query.FechaDesde,
            query.FechaHasta,
            query.Codigo,
            query.Nombre,
            query.TipoPeso,
            query.PesoKg,
            query.VacunoId,
            _dateTimeProvider.ServerNow);

        return new GenerateTriajesPdfOutput(
            _pdfGeneratorService.GenerateTriajesReport(document),
            "application/pdf",
            $"reporte-triajes-{document.GeneratedAtUtc:yyyyMMddHHmmss}.pdf");
    }
}
