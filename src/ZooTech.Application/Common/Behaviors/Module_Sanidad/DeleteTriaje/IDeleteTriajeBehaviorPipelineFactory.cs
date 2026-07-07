using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.DeleteTriaje
{
    public interface IDeleteTriajeBehaviorPipelineFactory
    {
        BehaviorPipeline<DeleteTriajeCommand , EmptyOutput> Create();
    }
}