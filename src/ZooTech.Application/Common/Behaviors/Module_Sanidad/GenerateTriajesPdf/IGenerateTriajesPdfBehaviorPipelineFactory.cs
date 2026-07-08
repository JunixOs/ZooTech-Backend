using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesPdf
{
    public interface IGenerateTriajesPdfBehaviorPipelineFactory
    {
        BehaviorPipeline<GenerateTriajesPdfQuery , GenerateTriajesPdfOutput> Create();
    }
}