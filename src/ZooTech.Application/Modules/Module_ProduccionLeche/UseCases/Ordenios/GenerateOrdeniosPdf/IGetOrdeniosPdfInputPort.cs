
namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

public interface IGetOrdeniosPdfInputPort
{
    Task<GenerateOrdeniosPdfOutput> Handle(GenerateOrdeniosComparationPdfQuery query, CancellationToken cancellationToken);

}
