using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.UpdateCelo
{
    public interface IUpdateCeloBehaviorPipelineFactory
    {
        BehaviorPipeline<UpdateCeloCommand , UpdateCeloOutput> Create();
    }
}