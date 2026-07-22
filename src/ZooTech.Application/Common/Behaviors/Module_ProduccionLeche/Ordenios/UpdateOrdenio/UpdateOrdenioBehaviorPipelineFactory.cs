using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.UpdateOrdenio
{
    public class UpdateOrdenioBehaviorPipelineFactory : IUpdateOrdenioBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<UpdateOrdenioCommand , UpdateOrdenioOutput> _validator;
        private readonly LoggingBehavior<UpdateOrdenioCommand , UpdateOrdenioOutput> _logging;
        private readonly AuditBehavior<UpdateOrdenioCommand , UpdateOrdenioOutput> _audit;

        private readonly IUpdateOrdenioInputPort _handler;

        public UpdateOrdenioBehaviorPipelineFactory(
            ValidationBehavior<UpdateOrdenioCommand , UpdateOrdenioOutput> validator,
            LoggingBehavior<UpdateOrdenioCommand , UpdateOrdenioOutput> logging,
            AuditBehavior<UpdateOrdenioCommand , UpdateOrdenioOutput> audit,

            IUpdateOrdenioInputPort handler
        )
        {
            _validator = validator;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<UpdateOrdenioCommand , UpdateOrdenioOutput> Create()
        {
            return new BehaviorPipeline<UpdateOrdenioCommand , UpdateOrdenioOutput>(
            [
                _validator,
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}