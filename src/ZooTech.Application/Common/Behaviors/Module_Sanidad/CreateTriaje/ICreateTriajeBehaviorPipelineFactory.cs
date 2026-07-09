using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.CreateTriaje
{
    public interface ICreateTriajeBehaviorPipelineFactory
    {
        BehaviorPipeline<CreateTriajeCommand , CreateTriajeOutput> Create();
    }
}