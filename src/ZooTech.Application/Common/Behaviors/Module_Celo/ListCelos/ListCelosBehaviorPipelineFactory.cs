using ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.ListCelos
{
    public class ListCelosBehaviorPipelineFactory : IListCelosBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<ListCelosCommand , ListCelosOutput> _logging;
        private readonly AuditBehavior<ListCelosCommand , ListCelosOutput> _audit;

        private readonly IListCelosInputPort _handler;

        public ListCelosBehaviorPipelineFactory(
            LoggingBehavior<ListCelosCommand , ListCelosOutput> logging,
            AuditBehavior<ListCelosCommand , ListCelosOutput> audit,

            IListCelosInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<ListCelosCommand , ListCelosOutput> Create()
        {
            return new BehaviorPipeline<ListCelosCommand , ListCelosOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}