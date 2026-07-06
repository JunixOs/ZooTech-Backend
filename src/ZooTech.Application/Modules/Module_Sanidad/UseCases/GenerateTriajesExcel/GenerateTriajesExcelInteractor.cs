using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

internal sealed class GenerateTriajesExcelInteractor : IGenerateTriajesExcelInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly IExcelGeneratorService _excelGeneratorService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GenerateTriajesExcelInteractor(
        ITriajeRepository repository,
        IExcelGeneratorService excelGeneratorService,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _excelGeneratorService = excelGeneratorService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GenerateTriajesExcelOutput> HandleAsync(
        GenerateTriajesExcelQuery query,
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
            cancellationToken);

        var document = new GenerateTriajesExcelDocument(
            triajes.Select(TriajeMapper.ToOutput).ToList(),
            query.Fecha,
            query.Codigo,
            query.Nombre,
            query.TipoPeso,
            query.PesoKg,
            _dateTimeProvider.ServerNow);

        return new GenerateTriajesExcelOutput(
            _excelGeneratorService.GenerateTriajesReport(document),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"reporte-triajes-{document.GeneratedAtUtc:yyyyMMddHHmmss}.xlsx");
    }
}
