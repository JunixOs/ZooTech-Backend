using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

public interface IGenerateTriajesPdfInputPort
    : IRequestHandler<GenerateTriajesPdfQuery , GenerateTriajesPdfOutput>
{
}
