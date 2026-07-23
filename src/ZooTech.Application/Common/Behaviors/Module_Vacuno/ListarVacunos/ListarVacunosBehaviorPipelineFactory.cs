using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ListarVacunos
{
    public class ListarVacunosBehaviorPipelineFactory : IListarVacunosBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<ListarVacunosCommand , ListarVacunosOutput> _validation;
        private readonly LoggingBehavior<ListarVacunosCommand , ListarVacunosOutput> _logging;
        private readonly AuditBehavior<ListarVacunosCommand , ListarVacunosOutput> _audit;
        private readonly QueryCacheBehavior<ListarVacunosCommand, ListarVacunosOutput> _queryCache;

        private readonly IListarVacunosInputPort _handler;

        public ListarVacunosBehaviorPipelineFactory(
            ValidationBehavior<ListarVacunosCommand , ListarVacunosOutput> validation,
            LoggingBehavior<ListarVacunosCommand , ListarVacunosOutput> logging,
            AuditBehavior<ListarVacunosCommand , ListarVacunosOutput> audit,
            QueryCacheBehavior<ListarVacunosCommand, ListarVacunosOutput> queryCache,
            IListarVacunosInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;
            _queryCache = queryCache;

            _handler = handler;
        }

        public BehaviorPipeline<ListarVacunosCommand , ListarVacunosOutput> Create()
        {
            return new BehaviorPipeline<ListarVacunosCommand , ListarVacunosOutput>(
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