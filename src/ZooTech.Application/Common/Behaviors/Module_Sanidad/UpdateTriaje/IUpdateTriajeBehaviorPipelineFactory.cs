using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.UpdateTriaje
{
    public interface IUpdateTriajeBehaviorPipelineFactory
    {
        BehaviorPipeline<UpdateTriajeCommand , UpdateTriajeOutput> Create();
    }
}