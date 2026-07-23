using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.CreateFecundacion
{
    public class CreateFecundacionBehaviorPipelineFactory : ICreateFecundacionBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<CreateFecundacionCommand , CreateFecundacionOutput> _validation;
        private readonly LoggingBehavior<CreateFecundacionCommand , CreateFecundacionOutput> _logging;
        private readonly AuditBehavior<CreateFecundacionCommand , CreateFecundacionOutput> _audit;
        private readonly EvictCacheBehavior<CreateFecundacionCommand, CreateFecundacionOutput> _evictCache;

        private readonly ICreateFecundacionInputPort _handler;

        public CreateFecundacionBehaviorPipelineFactory(
            ValidationBehavior<CreateFecundacionCommand , CreateFecundacionOutput> validation,
            LoggingBehavior<CreateFecundacionCommand , CreateFecundacionOutput> logging,
            AuditBehavior<CreateFecundacionCommand , CreateFecundacionOutput> audit,
            EvictCacheBehavior<CreateFecundacionCommand, CreateFecundacionOutput> evictCache,
            ICreateFecundacionInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;
            _evictCache = evictCache;

            _handler = handler;
        }

        public BehaviorPipeline<CreateFecundacionCommand , CreateFecundacionOutput> Create()
        {
            return new BehaviorPipeline<CreateFecundacionCommand , CreateFecundacionOutput>(
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