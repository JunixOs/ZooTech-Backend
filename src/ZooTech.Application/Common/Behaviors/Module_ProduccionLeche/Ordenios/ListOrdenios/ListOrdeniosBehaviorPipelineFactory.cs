using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.ListOrdenios
{
    public class ListOrdeniosBehaviorPipelineFactory : IListOrdeniosBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<ListOrdeniosQuery , ListOrdeniosOutput> _logging;
        private readonly AuditBehavior<ListOrdeniosQuery , ListOrdeniosOutput> _audit;

        private readonly IListOrdeniosInputPort _handler;

        public ListOrdeniosBehaviorPipelineFactory(
            LoggingBehavior<ListOrdeniosQuery , ListOrdeniosOutput> logging,
            AuditBehavior<ListOrdeniosQuery , ListOrdeniosOutput> audit,

            IListOrdeniosInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<ListOrdeniosQuery , ListOrdeniosOutput> Create()
        {
            return new BehaviorPipeline<ListOrdeniosQuery , ListOrdeniosOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}