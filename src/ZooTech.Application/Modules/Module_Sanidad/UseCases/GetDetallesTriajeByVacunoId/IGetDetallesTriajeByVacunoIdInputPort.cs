using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId
{
    public interface IGetDetallesTriajeByVacunoIdInputPort
        : IRequestHandler<GetDetallesTriajeByVacunoIdQuery , GetDetallesTriajeByVacunoIdOutput>
    {
    }
}