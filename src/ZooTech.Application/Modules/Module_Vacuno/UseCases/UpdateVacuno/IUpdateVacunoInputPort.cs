using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

public interface IUpdateVacunoInputPort
    : IRequestHandler<UpdateVacunoCommand , UpdateVacunoOutput>
{
}
