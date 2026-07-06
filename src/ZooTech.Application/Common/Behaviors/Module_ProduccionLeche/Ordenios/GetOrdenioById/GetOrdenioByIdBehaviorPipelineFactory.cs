using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GetOrdenioById
{
    public class GetOrdenioByIdBehaviorPipelineFactory : IGetOrdenioByIdBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetOrdenioByIdCommand , GetOrdenioByIdOutput> _logging;
        private readonly AuditBehavior<GetOrdenioByIdCommand , GetOrdenioByIdOutput> _audit;

        private readonly IGetOrdenioByIdInputPort _handler;

        public GetOrdenioByIdBehaviorPipelineFactory(
            LoggingBehavior<GetOrdenioByIdCommand , GetOrdenioByIdOutput> logging,
            AuditBehavior<GetOrdenioByIdCommand , GetOrdenioByIdOutput> audit,

            IGetOrdenioByIdInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetOrdenioByIdCommand , GetOrdenioByIdOutput> Create()
        {
            return new BehaviorPipeline<GetOrdenioByIdCommand , GetOrdenioByIdOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}