using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;

public interface IGetHistorialCeloPorVacunoInputPort
    : IRequestHandler<GetHistorialCeloPorVacunoCommand , GetHistorialCeloPorVacunoOutput>
{
}
