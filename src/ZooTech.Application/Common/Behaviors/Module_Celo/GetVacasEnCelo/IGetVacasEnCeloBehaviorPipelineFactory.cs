using ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetVacasEnCelo
{
    public interface IGetVacasEnCeloBehaviorPipelineFactory
    {
        BehaviorPipeline<GetVacasEnCeloCommand , GetVacasEnCeloOutput> Create();
    }
}