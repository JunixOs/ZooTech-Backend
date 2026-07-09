using ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.SearchFecundacionVacunos
{
    public class SearchFecundacionVacunosBehaviorPipelineFactory : ISearchFecundacionVacunosBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<SearchFecundacionVacunosQuery , IReadOnlyList<SearchFecundacionVacunoOutput>> _logging;
        private readonly AuditBehavior<SearchFecundacionVacunosQuery , IReadOnlyList<SearchFecundacionVacunoOutput>> _audit;

        private readonly ISearchFecundacionVacunosInputPort _handler;

        public SearchFecundacionVacunosBehaviorPipelineFactory(
            LoggingBehavior<SearchFecundacionVacunosQuery , IReadOnlyList<SearchFecundacionVacunoOutput>> logging,
            AuditBehavior<SearchFecundacionVacunosQuery , IReadOnlyList<SearchFecundacionVacunoOutput>> audit,

            ISearchFecundacionVacunosInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<SearchFecundacionVacunosQuery , IReadOnlyList<SearchFecundacionVacunoOutput>> Create()
        {
            return new BehaviorPipeline<SearchFecundacionVacunosQuery , IReadOnlyList<SearchFecundacionVacunoOutput>>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}