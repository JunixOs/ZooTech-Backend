using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

internal sealed class GenerateOrdeniosExcelInteractor : IGetOrdeniosExcelInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IExcelGeneratorService _excelGeneratorService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GenerateOrdeniosExcelInteractor(
        IOrdenioRepository repository,
        IExcelGeneratorService excelGeneratorService,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _excelGeneratorService = excelGeneratorService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GenerateOrdeniosExcelOutput> HandleAsync(
        GenerateOrdeniosComparationExcelQuery query,
        CancellationToken cancellationToken)
    {
        var entities = await _repository.ListReportAsync(
            query.VacunoId,
            query.EstadoOrdenioCode,
            query.FechaDesde,
            query.FechaHasta,
            cancellationToken);

        var document = new GenerateOrdeniosExcelDocument(
            entities.Select(OrdenioMapper.ToOutputList).ToList(),
            query.VacunoId,
            query.EstadoOrdenioCode,
            query.FechaDesde,
            query.FechaHasta,
            _dateTimeProvider.ServerNow);

        return new GenerateOrdeniosExcelOutput(
            _excelGeneratorService.GenerateOrdeniosReport(document),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"reporte-ordenios-{document.GeneratedAtUtc:yyyyMMddHHmmss}.xlsx");
    }


}
