using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

public interface IDeleteCeloInputPort
    : IRequestHandler<DeleteCeloCommand , EmptyOutput>
{
}
