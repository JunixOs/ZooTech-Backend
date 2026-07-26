
using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

public interface IGetOrdeniosPdfInputPort
    : IRequestHandler<GenerateOrdeniosComparationPdfQuery , GenerateOrdeniosPdfOutput>
{
}
