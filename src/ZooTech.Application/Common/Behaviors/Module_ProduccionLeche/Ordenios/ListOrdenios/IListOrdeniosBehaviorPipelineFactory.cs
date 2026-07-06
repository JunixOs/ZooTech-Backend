using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.ListOrdenios
{
    public interface IListOrdeniosBehaviorPipelineFactory
    {
        BehaviorPipeline<ListOrdeniosQuery , ListOrdeniosOutput> Create();
    }
}