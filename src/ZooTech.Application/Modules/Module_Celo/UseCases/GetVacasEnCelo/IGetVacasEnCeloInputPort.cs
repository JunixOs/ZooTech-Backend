using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;

public interface IGetVacasEnCeloInputPort
    : IRequestHandler<GetVacasEnCeloQuery , GetVacasEnCeloOutput>
{
}
