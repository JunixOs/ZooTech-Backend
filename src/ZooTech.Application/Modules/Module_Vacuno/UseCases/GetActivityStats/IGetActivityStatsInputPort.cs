using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

public interface IGetActivityStatsInputPort
    : IRequestHandler<GetActivityStatsQuery , GetActivityStatsOutput>
{
}
