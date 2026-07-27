using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;

public interface IListOrdeniosInputPort
    : IRequestHandler<ListOrdeniosQuery , ListOrdeniosOutput>
{
}
