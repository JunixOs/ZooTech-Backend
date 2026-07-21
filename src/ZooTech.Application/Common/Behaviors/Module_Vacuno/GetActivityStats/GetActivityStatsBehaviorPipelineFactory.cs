using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.GetActivityStats
{
    public class GetActivityStatsBehaviorPipelineFactory : IGetActivityStatsBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<GetActivityStatsQuery, GetActivityStatsOutput> _validation;
        private readonly LoggingBehavior<GetActivityStatsQuery, GetActivityStatsOutput> _logging;
        private readonly IGetActivityStatsInputPort _handler;

        public GetActivityStatsBehaviorPipelineFactory(
            ValidationBehavior<GetActivityStatsQuery, GetActivityStatsOutput> validation,
            LoggingBehavior<GetActivityStatsQuery, GetActivityStatsOutput> logging,
            IGetActivityStatsInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _handler = handler;
        }

        public BehaviorPipeline<GetActivityStatsQuery, GetActivityStatsOutput> Create()
        {
            return new BehaviorPipeline<GetActivityStatsQuery, GetActivityStatsOutput>(
            [
                _validation,
                _logging
            ],
            _handler.HandleAsync
            );
        }
    }
}
