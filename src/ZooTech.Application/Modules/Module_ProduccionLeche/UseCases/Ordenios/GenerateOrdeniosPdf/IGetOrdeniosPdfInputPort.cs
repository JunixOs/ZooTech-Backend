using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf.GeneratOrdenioComparationPdf;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

public interface IGetOrdeniosPdfInputPort
{
    Task<GenerateOrdeniosPdfOutput> HandleAsync(GenerateOrdeniosComparationPdfQuery query, CancellationToken cancellationToken);
    Task<GenerateOrdenioComparationPdfOutput> HandleComparationAsync(GenerateOrdeniosComparationPdfQuery query, CancellationToken cancellationToken);

}
