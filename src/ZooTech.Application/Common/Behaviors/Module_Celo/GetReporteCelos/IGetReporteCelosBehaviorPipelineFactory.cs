using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetReporteCelos
{
    public interface IGetReporteCelosBehaviorPipelineFactory
    {
        BehaviorPipeline<EmptyCommand, GetReporteCelosOutput> Create();
    }
}