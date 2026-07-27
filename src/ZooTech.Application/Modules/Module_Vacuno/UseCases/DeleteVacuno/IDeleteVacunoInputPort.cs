using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

public interface IDeleteVacunoInputPort
    : IRequestHandler<DeleteVacunoCommand , EmptyOutput>
{
}
