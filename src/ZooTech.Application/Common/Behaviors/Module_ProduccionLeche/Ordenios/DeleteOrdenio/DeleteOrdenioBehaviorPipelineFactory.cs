using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.DeleteOrdenio
{
    public class DeleteOrdenioBehaviorPipelineFactory : IDeleteOrdenioBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<DeleteOrdenioCommand , EmptyOutput> _validation;
        private readonly LoggingBehavior<DeleteOrdenioCommand , EmptyOutput> _logging;
        private readonly AuditBehavior<DeleteOrdenioCommand , EmptyOutput> _audit;

        private readonly IDeleteOrdenioInputPort _handler;

        public DeleteOrdenioBehaviorPipelineFactory(
            ValidationBehavior<DeleteOrdenioCommand , EmptyOutput> validation,
            LoggingBehavior<DeleteOrdenioCommand , EmptyOutput> logging,
            AuditBehavior<DeleteOrdenioCommand , EmptyOutput> audit,

            IDeleteOrdenioInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<DeleteOrdenioCommand , EmptyOutput> Create()
        {
            return new BehaviorPipeline<DeleteOrdenioCommand , EmptyOutput>(
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