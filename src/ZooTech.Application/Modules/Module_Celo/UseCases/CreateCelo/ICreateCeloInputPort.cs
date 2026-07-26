using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

public interface ICreateCeloInputPort
    : IRequestHandler<CreateCeloCommand , CreateCeloOutput>
{
}
