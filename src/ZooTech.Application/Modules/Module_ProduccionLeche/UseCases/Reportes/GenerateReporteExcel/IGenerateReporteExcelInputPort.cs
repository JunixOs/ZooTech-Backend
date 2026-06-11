namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReporteExcel;

public interface IGenerateReporteExcelInputPort
{
    Task<GenerateReporteExcelOutput> HandleAsync(GenerateReporteExcelQuery query, CancellationToken cancellationToken);
}
