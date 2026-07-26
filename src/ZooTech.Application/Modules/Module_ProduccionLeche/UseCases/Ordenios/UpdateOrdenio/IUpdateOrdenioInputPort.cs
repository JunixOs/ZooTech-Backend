using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

public interface IUpdateOrdenioInputPort
    : IRequestHandler<UpdateOrdenioCommand , UpdateOrdenioOutput>
{
}
