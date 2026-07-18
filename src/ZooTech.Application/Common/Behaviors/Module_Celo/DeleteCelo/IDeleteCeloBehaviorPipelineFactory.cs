using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.DeleteCelo
{
    public interface IDeleteCeloBehaviorPipelineFactory
    {
        BehaviorPipeline<DeleteCeloCommand, EmptyOutput> Create();
    }
}