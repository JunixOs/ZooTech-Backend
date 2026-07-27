using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;

public interface IGetOrdenioByIdInputPort
    : IRequestHandler<GetOrdenioByIdQuery , GetOrdenioByIdOutput>
{
}
