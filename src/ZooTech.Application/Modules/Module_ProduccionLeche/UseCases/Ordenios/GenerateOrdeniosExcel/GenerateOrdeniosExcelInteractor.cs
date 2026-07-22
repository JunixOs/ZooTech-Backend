using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

internal sealed class GenerateOrdeniosExcelInteractor : IGetOrdeniosExcelInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IExcelGeneratorService _excelGeneratorService;
    private readonly IOrdeniosComparationExcelGeneratorService _comparationExcelGeneratorService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GenerateOrdeniosExcelInteractor(
        IOrdenioRepository repository,
        IExcelGeneratorService excelGeneratorService,
        IOrdeniosComparationExcelGeneratorService comparationExcelGeneratorService,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _excelGeneratorService = excelGeneratorService;
        _comparationExcelGeneratorService = comparationExcelGeneratorService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GenerateOrdeniosExcelOutput> Handle(
        GenerateOrdeniosComparationExcelQuery query,
        CancellationToken cancellationToken
    )
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

        var excelContent = query.Comparativo
            ? _comparationExcelGeneratorService.GenerateOrdeniosReport(document)
            : _excelGeneratorService.GenerateOrdeniosReport(document);

        var fileNamePrefix = query.Comparativo
            ? "reporte-comparativo-ordenios"
            : "reporte-ordenios";

        return new GenerateOrdeniosExcelOutput(
            excelContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"{fileNamePrefix}-{document.GeneratedAtUtc:yyyyMMddHHmmss}.xlsx");
    }


}
