using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.UpdateCelo
{
    public class UpdateCeloBehaviorPipelineFactory : IUpdateCeloBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<UpdateCeloCommand , UpdateCeloOutput> _validation;
        private readonly LoggingBehavior<UpdateCeloCommand , UpdateCeloOutput> _logging;
        private readonly AuditBehavior<UpdateCeloCommand , UpdateCeloOutput> _audit;

        private readonly IUpdateCeloInputPort _handler;

        public UpdateCeloBehaviorPipelineFactory(
            ValidationBehavior<UpdateCeloCommand , UpdateCeloOutput> validation,
            LoggingBehavior<UpdateCeloCommand , UpdateCeloOutput> logging,
            AuditBehavior<UpdateCeloCommand , UpdateCeloOutput> audit,

            IUpdateCeloInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<UpdateCeloCommand , UpdateCeloOutput> Create()
        {
            return new BehaviorPipeline<UpdateCeloCommand , UpdateCeloOutput>(
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