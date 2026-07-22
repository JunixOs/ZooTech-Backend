using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

internal sealed class GenerateOrdeniosExcelInteractor : IGetOrdeniosExcelInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IExcelGeneratorService _excelGeneratorService;
    private readonly IExcelComparationGeneratorService _excelComparationGeneratorService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GenerateOrdeniosExcelInteractor(
        IOrdenioRepository repository,
        IExcelGeneratorService excelGeneratorService,
        IExcelComparationGeneratorService excelComparationGeneratorService,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _excelGeneratorService = excelGeneratorService;
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

        if (query.Comparativo)
        {
            var comparativoRows = entities.Select(e => new ExcelComparationRow(
                e.FechaHora,
                e.Litros,
                e.Litros
            )).ToList();
            var comparativoRequest = new ExcelComparationReportRequest(
                Title: "Comparación de Producción Real vs. Estándar",
                SheetName: "Comparativo",
                Rows: comparativoRows,
                GeneratedAtUtc: _dateTimeProvider.ServerNow,
                FilterLine: $"Desde: {query.FechaDesde:yyyy-MM-dd} | Hasta: {query.FechaHasta:yyyy-MM-dd}",
                SerieRealLabel: "Producción Real (L)",
                SerieComparacionLabel: "Producción Estándar (L)"
            );
            return new GenerateOrdeniosExcelOutput(
                _excelComparationGeneratorService.Generate(comparativoRequest),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"comparativo-produccion-{_dateTimeProvider.ServerNow:yyyyMMddHHmmss}.xlsx");
        }

        // Columnas por defecto si no se especifican en el query
        var columns = query.Columns ?? new List<ExcelColumnDefinition>{
            new("Código Registro", "Codigo"),
            new("Fecha", "Fecha"),
            new("Hora", "Hora"),
            new("Vacuno", "NombreVacuno"),
            new("Código Vacuno", "VacunoCodigo"),
            new("Litros", "Litros"),
        };
        

        var document = new GenerateOrdeniosExcelDocument(
            entities.Select(OrdenioMapper.ToOutputList).ToList(),
            columns,
            query.SheetName,
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
