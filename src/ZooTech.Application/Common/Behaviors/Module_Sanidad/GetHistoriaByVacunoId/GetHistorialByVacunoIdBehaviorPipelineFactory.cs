using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialByVacunoId
{
    public class GetHistorialByVacunoIdBehaviorPipelineFactory : IGetHistorialByVacunoIdBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetHistorialByVacunoIdCommand , GetHistorialByVacunoIdOutput> _logging;
        private readonly AuditBehavior<GetHistorialByVacunoIdCommand , GetHistorialByVacunoIdOutput> _audit;

        private readonly IGetHistorialByVacunoIdInputPort _handler;

        public GetHistorialByVacunoIdBehaviorPipelineFactory(
            LoggingBehavior<GetHistorialByVacunoIdCommand , GetHistorialByVacunoIdOutput> logging,
            AuditBehavior<GetHistorialByVacunoIdCommand , GetHistorialByVacunoIdOutput> audit,

            IGetHistorialByVacunoIdInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetHistorialByVacunoIdCommand , GetHistorialByVacunoIdOutput> Create()
        {
            return new BehaviorPipeline<GetHistorialByVacunoIdCommand , GetHistorialByVacunoIdOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}