using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetDetallesTriajeByVacunoId
{
    public class GetDetallesTriajeByVacunoIdBehaviorPipelineFactory : IGetDetallesTriajeByVacunoIdBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetDetallesTriajeByVacunoIdCommand , GetDetallesTriajeByVacunoIdOutput> _logging;
        private readonly AuditBehavior<GetDetallesTriajeByVacunoIdCommand , GetDetallesTriajeByVacunoIdOutput> _audit;

        private readonly IGetDetallesTriajeByVacunoIdInputPort _handler;

        public GetDetallesTriajeByVacunoIdBehaviorPipelineFactory(
            LoggingBehavior<GetDetallesTriajeByVacunoIdCommand , GetDetallesTriajeByVacunoIdOutput> logging,
            AuditBehavior<GetDetallesTriajeByVacunoIdCommand , GetDetallesTriajeByVacunoIdOutput> audit,

            IGetDetallesTriajeByVacunoIdInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetDetallesTriajeByVacunoIdCommand , GetDetallesTriajeByVacunoIdOutput> Create()
        {
            return new BehaviorPipeline<GetDetallesTriajeByVacunoIdCommand , GetDetallesTriajeByVacunoIdOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}