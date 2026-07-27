using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

public interface IGetHistorialByVacunoIdInputPort
    : IRequestHandler<GetHistorialByVacunoIdQuery , GetHistorialByVacunoIdOutput>
{
}
