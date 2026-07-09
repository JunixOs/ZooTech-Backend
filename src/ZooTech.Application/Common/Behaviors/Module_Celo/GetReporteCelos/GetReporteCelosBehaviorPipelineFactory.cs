using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetReporteCelos
{
    public class GetReporteCelosBehaviorPipelineFactory : IGetReporteCelosBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommand , GetReporteCelosOutput> _logging;
        private readonly AuditBehavior<EmptyCommand , GetReporteCelosOutput> _audit;

        private readonly IGetReporteCelosInputPort _handler;

        public GetReporteCelosBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommand , GetReporteCelosOutput> logging,
            AuditBehavior<EmptyCommand , GetReporteCelosOutput> audit,

            IGetReporteCelosInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommand , GetReporteCelosOutput> Create()
        {
            return new BehaviorPipeline<EmptyCommand , GetReporteCelosOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}