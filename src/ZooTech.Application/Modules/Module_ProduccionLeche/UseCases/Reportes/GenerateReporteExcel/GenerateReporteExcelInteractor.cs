using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Models.Reports;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;
using ZooTech.Domain.Entities.Configuration;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReporteExcel;

public class GenerateReporteExcelInteractor : IGenerateReporteExcelInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IExcelGeneratorService _excelGenerator;

    public GenerateReporteExcelInteractor(IOrdenioRepository repository, IExcelGeneratorService excelGenerator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _excelGenerator = excelGenerator ?? throw new ArgumentNullException(nameof(excelGenerator));
    }

    public async Task<GenerateReporteExcelOutput> HandleAsync(GenerateReporteExcelQuery query, CancellationToken cancellationToken)
    {
        var items = await _repository.GetProduccionDiariaAsync(
            query.FechaDesde,
            query.FechaHasta,
            query.VacunoId,
            cancellationToken);

        var reporte = new ReporteProduccionDiaria
        {
            Metadata = new ReportMetadata
            {
                Title = ConfigSettings.Reporteleche.ReportDailyProductionTitle,
                GeneratedAt = DateTime.Now,
                Company = ConfigSettings.Reporteleche.ReportCompanyName,
                Department = ConfigSettings.Reporteleche.ReportDepartmentName
            },
            VacunoId = query.VacunoId,
            FechaDesde = query.FechaDesde,
            FechaHasta = query.FechaHasta,
            Items = items
                .Select(x => new ProduccionDiariaItemReport
                {
                    Fecha = x.Fecha,
                    TotalLitros = x.TotalLitros,
                    CantidadOrdenios = x.CantidadOrdenios
                })
                .ToList()
        };

        var excelBytes = await _excelGenerator.GenerateReporteProduccionDiariaAsync(reporte, cancellationToken);
        var fileName = GenerarNombreArchivo(query);

        return new GenerateReporteExcelOutput(excelBytes, fileName);
    }

    private static string GenerarNombreArchivo(GenerateReporteExcelQuery query)
    {
        var fechaDesde = query.FechaDesde?.ToString(ConfigSettings.Reporteleche.ReportFileDatetimeFormat);
        var fechaHasta = query.FechaHasta?.ToString(ConfigSettings.Reporteleche.ReportFileDatetimeFormat);
        var vacunoId = query.VacunoId.HasValue ? $"_{ConfigSettings.Reporteleche.ReportFileCattlePrefix}_{query.VacunoId}" : string.Empty;
        var fechaNow = DateTime.Now.ToString(ConfigSettings.Reporteleche.ReportFileDatetimeFormat);
        var name = ConfigSettings.Reporteleche.ReportFileBaseName;

        if (string.IsNullOrEmpty(fechaDesde) && string.IsNullOrEmpty(fechaHasta))
        {
            return $"{name}{vacunoId}_{fechaNow}.xlsx";
        }

        return $"{name}_{fechaDesde}_{fechaHasta}{vacunoId}_{fechaNow}.xlsx";
    }
}
