using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.GetActivityStats
{
    public interface IGetActivityStatsBehaviorPipelineFactory
    {
        BehaviorPipeline<GetActivityStatsQuery, GetActivityStatsOutput> Create();
    }
}
