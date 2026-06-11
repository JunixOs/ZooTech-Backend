using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Models.Reports;
using ZooTech.Application.Common.Models.Reports.Builders;

namespace ZooTech.Infrastructure.Common.Services.ExcelGenerator;

public class ExcelGeneratorService : IExcelGeneratorService
{
    public byte[] GenerateReporteProduccionDiaria(ReporteProduccionDiaria reporte)
    {
        var builder = new ProduccionDiariaExcelBuilder(reporte);
        return builder.Build();
    }

    public async Task<byte[]> GenerateReporteProduccionDiariaAsync(
        ReporteProduccionDiaria reporte,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => GenerateReporteProduccionDiaria(reporte), cancellationToken);
    }
}
