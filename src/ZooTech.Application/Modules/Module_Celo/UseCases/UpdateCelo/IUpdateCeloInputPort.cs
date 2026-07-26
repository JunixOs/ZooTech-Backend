using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

public interface IUpdateCeloInputPort
    : IRequestHandler<UpdateCeloCommand , UpdateCeloOutput>
{
}
