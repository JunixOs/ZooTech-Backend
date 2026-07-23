using ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.ListarFecundacion
{
    public class ListarFecundacionBehaviorPipelineFactory : IListarFecundacionBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<ListarFecundacionCommand , ListarFecundacionOutput> _validation;
        private readonly LoggingBehavior<ListarFecundacionCommand , ListarFecundacionOutput> _logging;
        private readonly AuditBehavior<ListarFecundacionCommand , ListarFecundacionOutput> _audit;
        private readonly QueryCacheBehavior<ListarFecundacionCommand, ListarFecundacionOutput> _queryCache;

        private readonly IListarFecundacionInputPort _handler;

        public ListarFecundacionBehaviorPipelineFactory(
            ValidationBehavior<ListarFecundacionCommand , ListarFecundacionOutput> validation,
            LoggingBehavior<ListarFecundacionCommand , ListarFecundacionOutput> logging,
            AuditBehavior<ListarFecundacionCommand , ListarFecundacionOutput> audit,
            QueryCacheBehavior<ListarFecundacionCommand, ListarFecundacionOutput> queryCache,
            IListarFecundacionInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;
            _queryCache = queryCache;

            _handler = handler;
        }

        public BehaviorPipeline<ListarFecundacionCommand , ListarFecundacionOutput> Create()
        {
            return new BehaviorPipeline<ListarFecundacionCommand , ListarFecundacionOutput>(
            [
                _validation,
                _logging,
                _audit,
                _queryCache
            ],
            _handler.HandleAsync
            );
        }
    }
}