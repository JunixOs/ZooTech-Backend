using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;

public interface IGetVacunoByIdInputPort
    : IRequestHandler<GetVacunoByIdQuery , GetVacunoByIdOutput>
{
}
