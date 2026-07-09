using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialByVacunoId
{
    public interface IGetHistorialByVacunoIdBehaviorPipelineFactory
    {
        BehaviorPipeline<GetHistorialByVacunoIdCommand , GetHistorialByVacunoIdOutput> Create();
    }
}