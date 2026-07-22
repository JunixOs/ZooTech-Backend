using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

namespace ZooTech.Application.Common.Gateway.Services;

public interface IPdfGeneratorService
{
    byte[] GenerateOrdeniosReport(GenerateOrdeniosPdfDocument document);
    byte[] GenerateTriajesReport(GenerateTriajesPdfDocument document);
}
