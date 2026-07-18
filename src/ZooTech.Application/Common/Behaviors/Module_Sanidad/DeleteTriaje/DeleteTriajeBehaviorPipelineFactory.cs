using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.DeleteTriaje
{
    public class DeleteTriajeBehaviorPipelineFactory : IDeleteTriajeBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<DeleteTriajeCommand , EmptyOutput> _validator;
        private readonly LoggingBehavior<DeleteTriajeCommand , EmptyOutput> _logging;
        private readonly AuditBehavior<DeleteTriajeCommand , EmptyOutput> _audit;

        private readonly IDeleteTriajeInputPort _handler;

        public DeleteTriajeBehaviorPipelineFactory(
            ValidationBehavior<DeleteTriajeCommand , EmptyOutput> validator,
            LoggingBehavior<DeleteTriajeCommand , EmptyOutput> logging,
            AuditBehavior<DeleteTriajeCommand , EmptyOutput> audit,

            IDeleteTriajeInputPort handler
        )
        {
            _validator = validator;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<DeleteTriajeCommand , EmptyOutput> Create()
        {
            return new BehaviorPipeline<DeleteTriajeCommand , EmptyOutput>(
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