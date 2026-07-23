using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.DeleteFecundacion
{
    public class DeleteFecundacionBehaviorPipelineFactory : IDeleteFecundacionBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<DeleteFecundacionCommand, EmptyOutput> _validation;
        private readonly LoggingBehavior<DeleteFecundacionCommand , EmptyOutput> _logging;
        private readonly AuditBehavior<DeleteFecundacionCommand , EmptyOutput> _audit;
        private readonly EvictCacheBehavior<DeleteFecundacionCommand, EmptyOutput> _evictCache;

        private readonly IDeleteFecundacionInputPort _handler;

        public DeleteFecundacionBehaviorPipelineFactory(
            ValidationBehavior<DeleteFecundacionCommand, EmptyOutput> validation,
            LoggingBehavior<DeleteFecundacionCommand , EmptyOutput> logging,
            AuditBehavior<DeleteFecundacionCommand , EmptyOutput> audit,
            EvictCacheBehavior<DeleteFecundacionCommand, EmptyOutput> evictCache,
            IDeleteFecundacionInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;
            _evictCache = evictCache;

            _handler = handler;
        }

        public BehaviorPipeline<DeleteFecundacionCommand , EmptyOutput> Create()
        {
            return new BehaviorPipeline<DeleteFecundacionCommand , EmptyOutput>(
            [
                _validation,
                _logging,
                _audit,
                _evictCache
            ],
            _handler.HandleAsync
            );
        }
    }
}
