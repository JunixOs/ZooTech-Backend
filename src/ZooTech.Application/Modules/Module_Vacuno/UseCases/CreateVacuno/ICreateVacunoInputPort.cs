using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

public interface ICreateVacunoInputPort
    : IRequestHandler<CreateVacunoCommand , CreateVacunoOutput>
{
}
