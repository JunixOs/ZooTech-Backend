using ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.ListCelos
{
    public interface IListCelosBehaviorPipelineFactory
    {
        BehaviorPipeline<ListCelosCommand , ListCelosOutput> Create();
    }
}