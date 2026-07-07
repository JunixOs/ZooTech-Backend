using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

namespace ZooTech.Application.Common.Gateway.Services;

public interface IOrdeniosComparationPdfGeneratorService
{
    byte[] GenerateOrdeniosReport(GenerateOrdeniosPdfDocument document);
}
