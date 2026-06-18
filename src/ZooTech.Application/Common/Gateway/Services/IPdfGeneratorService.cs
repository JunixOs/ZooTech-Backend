using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf.GeneratOrdenioComparationPdf;

namespace ZooTech.Application.Common.Gateway.Services;

public interface IPdfGeneratorService
{
    byte[] GenerateOrdeniosReport(GenerateOrdeniosPdfDocument document);
    byte[] GenerateOrdenioComparationReport(GenerateOrdenioComparationPdfDocument document);
}
