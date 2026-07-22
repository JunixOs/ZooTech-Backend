using ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.ListReporteCeloGeneral
{
    public class ListReporteCeloGeneralBehaviorPipelineFactory : IListReporteCeloGeneralBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<ListReporteCeloGeneralCommand , ListReporteCeloGeneralOutput> _logging;
        private readonly AuditBehavior<ListReporteCeloGeneralCommand , ListReporteCeloGeneralOutput> _audit;

        private readonly IListReporteCeloGeneralInputPort _handler;

        public ListReporteCeloGeneralBehaviorPipelineFactory(
            LoggingBehavior<ListReporteCeloGeneralCommand , ListReporteCeloGeneralOutput> logging,
            AuditBehavior<ListReporteCeloGeneralCommand , ListReporteCeloGeneralOutput> audit,

            IListReporteCeloGeneralInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<ListReporteCeloGeneralCommand , ListReporteCeloGeneralOutput> Create()
        {
            return new BehaviorPipeline<ListReporteCeloGeneralCommand , ListReporteCeloGeneralOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}