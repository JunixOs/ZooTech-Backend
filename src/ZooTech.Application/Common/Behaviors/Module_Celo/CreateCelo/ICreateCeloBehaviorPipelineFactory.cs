using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.CreateCelo
{
    public interface ICreateCeloBehaviorPipelineFactory
    {
        BehaviorPipeline<CreateCeloCommand , CreateCeloOutput> Create();
    }
}