using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.UpdateOrdenio
{
    public interface IUpdateOrdenioBehaviorPipelineFactory
    {
        BehaviorPipeline<UpdateOrdenioCommand , UpdateOrdenioOutput> Create();
    }
}