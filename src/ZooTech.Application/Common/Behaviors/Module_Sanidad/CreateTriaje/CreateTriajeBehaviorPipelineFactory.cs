using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.CreateTriaje
{
    public class CreateTriajeBehaviorPipelineFactory : ICreateTriajeBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<CreateTriajeCommand , CreateTriajeOutput> _validator;
        private readonly LoggingBehavior<CreateTriajeCommand , CreateTriajeOutput> _logging;
        private readonly AuditBehavior<CreateTriajeCommand , CreateTriajeOutput> _audit;

        private readonly ICreateTriajeInputPort _handler;

        public CreateTriajeBehaviorPipelineFactory(
            ValidationBehavior<CreateTriajeCommand , CreateTriajeOutput> validator,
            LoggingBehavior<CreateTriajeCommand , CreateTriajeOutput> logging,
            AuditBehavior<CreateTriajeCommand , CreateTriajeOutput> audit,

            ICreateTriajeInputPort handler
        )
        {
            _validator = validator;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<CreateTriajeCommand , CreateTriajeOutput> Create()
        {
            return new BehaviorPipeline<CreateTriajeCommand , CreateTriajeOutput>(
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