using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ReporteVacuno.ListarVacunosReporte
{
    public class ListarVacunosReporteBehaviorPipelineFactory : IListarVacunosReporteBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<ListarVacunosReporteQuery , ListarVacunosReporteResponse> _logging;
        private readonly AuditBehavior<ListarVacunosReporteQuery , ListarVacunosReporteResponse> _audit;
        private readonly ValidationBehavior<ListarVacunosReporteQuery, ListarVacunosReporteResponse> _validation;

        private readonly IListarVacunosReporteUseCase _handler;

        public ListarVacunosReporteBehaviorPipelineFactory(
            LoggingBehavior<ListarVacunosReporteQuery , ListarVacunosReporteResponse> logging,
            AuditBehavior<ListarVacunosReporteQuery , ListarVacunosReporteResponse> audit,
            ValidationBehavior<ListarVacunosReporteQuery, ListarVacunosReporteResponse> validation,
            IListarVacunosReporteUseCase handler
        )
        {
            _logging = logging;
            _audit = audit;
            _validation = validation;
            _handler = handler;
        }

        public BehaviorPipeline<ListarVacunosReporteQuery , ListarVacunosReporteResponse> Create()
        {
            return new BehaviorPipeline<ListarVacunosReporteQuery , ListarVacunosReporteResponse>(
            [
                _validation,
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}
