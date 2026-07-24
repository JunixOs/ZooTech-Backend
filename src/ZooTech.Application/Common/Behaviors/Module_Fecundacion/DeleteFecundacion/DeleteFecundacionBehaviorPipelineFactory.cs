using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.DeleteFecundacion
{
    public class DeleteFecundacionBehaviorPipelineFactory : IDeleteFecundacionBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<DeleteFecundacionCommand , EmptyOutput> _logging;
        private readonly AuditBehavior<DeleteFecundacionCommand , EmptyOutput> _audit;

        private readonly IDeleteFecundacionInputPort _handler;

        public DeleteFecundacionBehaviorPipelineFactory(
            LoggingBehavior<DeleteFecundacionCommand , EmptyOutput> logging,
            AuditBehavior<DeleteFecundacionCommand , EmptyOutput> audit,

            IDeleteFecundacionInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<DeleteFecundacionCommand , EmptyOutput> Create()
        {
            return new BehaviorPipeline<DeleteFecundacionCommand , EmptyOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}