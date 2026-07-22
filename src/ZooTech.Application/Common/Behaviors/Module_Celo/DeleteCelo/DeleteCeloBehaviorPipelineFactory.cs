using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.DeleteCelo
{
    public class DeleteCeloBehaviorPipelineFactory : IDeleteCeloBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<DeleteCeloCommand, EmptyOutput> _validation;
        private readonly LoggingBehavior<DeleteCeloCommand, EmptyOutput> _logging;
        private readonly AuditBehavior<DeleteCeloCommand, EmptyOutput> _audit;

        private readonly IDeleteCeloInputPort _handler;

        public DeleteCeloBehaviorPipelineFactory(
            ValidationBehavior<DeleteCeloCommand, EmptyOutput> validation,
            LoggingBehavior<DeleteCeloCommand, EmptyOutput> logging,
            AuditBehavior<DeleteCeloCommand, EmptyOutput> audit,

            IDeleteCeloInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<DeleteCeloCommand, EmptyOutput> Create()
        {
            return new BehaviorPipeline<DeleteCeloCommand, EmptyOutput>(
            [
                _validation,
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}