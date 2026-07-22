using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTriajes
{
    public interface IGetAllTriajesBehaviorPipelineFactory
    {
        BehaviorPipeline<GetAllTriajesQuery , GetAllTriajesOutput> Create();
    }
}