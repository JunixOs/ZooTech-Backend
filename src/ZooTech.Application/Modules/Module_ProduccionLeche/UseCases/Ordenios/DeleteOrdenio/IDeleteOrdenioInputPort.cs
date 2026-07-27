using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

public interface IDeleteOrdenioInputPort
    : IRequestHandler<DeleteOrdenioCommand , EmptyOutput>
{
}
