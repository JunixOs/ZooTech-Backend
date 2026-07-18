using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.CreateCelo
{
    public class CreateCeloBehaviorPipelineFactory : ICreateCeloBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<CreateCeloCommand , CreateCeloOutput> _validation;
        private readonly LoggingBehavior<CreateCeloCommand, CreateCeloOutput> _logging;
        private readonly AuditBehavior<CreateCeloCommand, CreateCeloOutput> _audit;

        private readonly ICreateCeloInputPort _handler;

        public CreateCeloBehaviorPipelineFactory(
            ValidationBehavior<CreateCeloCommand , CreateCeloOutput> validation,
            LoggingBehavior<CreateCeloCommand, CreateCeloOutput> logging,
            AuditBehavior<CreateCeloCommand, CreateCeloOutput> audit,

            ICreateCeloInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<CreateCeloCommand, CreateCeloOutput> Create()
        {
            return new BehaviorPipeline<CreateCeloCommand, CreateCeloOutput>(
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