using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.UpdateTriaje
{
    public class UpdateTriajeBehaviorPipelineFactory : IUpdateTriajeBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<UpdateTriajeCommand , UpdateTriajeOutput> _validator;
        private readonly LoggingBehavior<UpdateTriajeCommand , UpdateTriajeOutput> _logging;
        private readonly AuditBehavior<UpdateTriajeCommand , UpdateTriajeOutput> _audit;

        private readonly IUpdateTriajeInputPort _handler;

        public UpdateTriajeBehaviorPipelineFactory(
            ValidationBehavior<UpdateTriajeCommand , UpdateTriajeOutput> validator,
            LoggingBehavior<UpdateTriajeCommand , UpdateTriajeOutput> logging,
            AuditBehavior<UpdateTriajeCommand , UpdateTriajeOutput> audit,

            IUpdateTriajeInputPort handler
        )
        {
            _validator = validator;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<UpdateTriajeCommand , UpdateTriajeOutput> Create()
        {
            return new BehaviorPipeline<UpdateTriajeCommand , UpdateTriajeOutput>(
            [
                _validator,
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}