using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.CreateOrdenio
{
    public class CreateOrdenioBehaviorPipelineFactory : ICreateOrdenioBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<CreateOrdenioCommand , CreateOrdenioOutput> _validation;
        private readonly LoggingBehavior<CreateOrdenioCommand , CreateOrdenioOutput> _logging;
        private readonly AuditBehavior<CreateOrdenioCommand , CreateOrdenioOutput> _audit;

        private readonly ICreateOrdenioInputPort _handler;

        public CreateOrdenioBehaviorPipelineFactory(
            ValidationBehavior<CreateOrdenioCommand , CreateOrdenioOutput> validation,
            LoggingBehavior<CreateOrdenioCommand , CreateOrdenioOutput> logging,
            AuditBehavior<CreateOrdenioCommand , CreateOrdenioOutput> audit,

            ICreateOrdenioInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<CreateOrdenioCommand , CreateOrdenioOutput> Create()
        {
            return new BehaviorPipeline<CreateOrdenioCommand , CreateOrdenioOutput>(
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