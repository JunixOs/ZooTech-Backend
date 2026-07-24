using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ReporteVacuno.ObtenerRegistroVacunoReporte
{
    public class ObtenerRegistroVacunoReporteBehaviorPipelineFactory : IObtenerRegistroVacunoReporteBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<ObtenerRegistroVacunoReporteQuery , RegistroVacunoReporteResponse> _logging;
        private readonly AuditBehavior<ObtenerRegistroVacunoReporteQuery , RegistroVacunoReporteResponse> _audit;

        private readonly IObtenerRegistroVacunoReporteUseCase _handler;

        public ObtenerRegistroVacunoReporteBehaviorPipelineFactory(
            LoggingBehavior<ObtenerRegistroVacunoReporteQuery , RegistroVacunoReporteResponse> logging,
            AuditBehavior<ObtenerRegistroVacunoReporteQuery , RegistroVacunoReporteResponse> audit,

            IObtenerRegistroVacunoReporteUseCase handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<ObtenerRegistroVacunoReporteQuery , RegistroVacunoReporteResponse> Create()
        {
            return new BehaviorPipeline<ObtenerRegistroVacunoReporteQuery , RegistroVacunoReporteResponse>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}