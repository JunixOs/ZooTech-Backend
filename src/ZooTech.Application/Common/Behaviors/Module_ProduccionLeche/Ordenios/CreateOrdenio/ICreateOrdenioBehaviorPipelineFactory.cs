using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.CreateOrdenio
{
    public interface ICreateOrdenioBehaviorPipelineFactory
    {
        BehaviorPipeline<CreateOrdenioCommand , CreateOrdenioOutput> Create();
    }
}