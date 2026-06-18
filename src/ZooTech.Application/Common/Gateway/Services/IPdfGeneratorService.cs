using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

namespace ZooTech.Application.Common.Gateway.Services;

public interface IPdfGeneratorService
{
    byte[] GenerateOrdeniosReport(GenerateOrdeniosPdfDocument document);
}
