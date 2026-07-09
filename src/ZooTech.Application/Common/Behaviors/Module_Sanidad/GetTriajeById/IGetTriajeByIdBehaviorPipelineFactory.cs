using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetTriajeById
{
    public interface IGetTriajeByIdBehaviorPipelineFactory
    {
        BehaviorPipeline<GetTriajeByIdCommand , GetTriajeByIdOutput> Create();
    }
}