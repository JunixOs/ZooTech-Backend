namespace ZooTech.Application.Common.Gateway.Services;

public interface IExcelGeneratorService
{
    byte[] GenerateReporteProduccionDiaria(Common.Models.Reports.ReporteProduccionDiaria reporte);

    Task<byte[]> GenerateReporteProduccionDiariaAsync(
        Common.Models.Reports.ReporteProduccionDiaria reporte,
        CancellationToken cancellationToken = default);
}
