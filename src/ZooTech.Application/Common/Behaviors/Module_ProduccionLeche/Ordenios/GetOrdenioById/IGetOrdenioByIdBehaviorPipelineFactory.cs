using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GetOrdenioById
{
    public interface IGetOrdenioByIdBehaviorPipelineFactory
    {
        BehaviorPipeline<GetOrdenioByIdCommand , GetOrdenioByIdOutput> Create();
    }
}