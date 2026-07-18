using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetCelos
{
    public interface IGetCelosBehaviorPipelineFactory
    {
        BehaviorPipeline<EmptyCommand , GetCelosOutput> Create();
    }
}