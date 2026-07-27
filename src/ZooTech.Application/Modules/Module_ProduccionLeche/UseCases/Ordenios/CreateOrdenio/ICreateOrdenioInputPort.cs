using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

public interface ICreateOrdenioInputPort
    : IRequestHandler<CreateOrdenioCommand , CreateOrdenioOutput>
{
}
