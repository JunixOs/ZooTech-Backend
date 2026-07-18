using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetTriajeById
{
    public class GetTriajeByIdBehaviorPipelineFactory : IGetTriajeByIdBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetTriajeByIdCommand , GetTriajeByIdOutput> _logging;
        private readonly AuditBehavior<GetTriajeByIdCommand , GetTriajeByIdOutput> _audit;

        private readonly IGetTriajeByIdInputPort _handler;

        public GetTriajeByIdBehaviorPipelineFactory(
            LoggingBehavior<GetTriajeByIdCommand , GetTriajeByIdOutput> logging,
            AuditBehavior<GetTriajeByIdCommand , GetTriajeByIdOutput> audit,

            IGetTriajeByIdInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetTriajeByIdCommand , GetTriajeByIdOutput> Create()
        {
            return new BehaviorPipeline<GetTriajeByIdCommand , GetTriajeByIdOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}