using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetDetallesTriajeByVacunoId
{
    public interface IGetDetallesTriajeByVacunoIdBehaviorPipelineFactory
    {
        BehaviorPipeline<GetDetallesTriajeByVacunoIdCommand , GetDetallesTriajeByVacunoIdOutput> Create();
    }
}