using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.CreateVacuno
{
    public class CreateVacunoBehaviorPipelineFactory : ICreateVacunoBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<CreateVacunoCommand , CreateVacunoOutput> _validation;
        private readonly LoggingBehavior<CreateVacunoCommand , CreateVacunoOutput> _logging;
        private readonly AuditBehavior<CreateVacunoCommand , CreateVacunoOutput> _audit;

        private readonly ICreateVacunoInputPort _handler;

        public CreateVacunoBehaviorPipelineFactory(
            ValidationBehavior<CreateVacunoCommand , CreateVacunoOutput> validation,
            LoggingBehavior<CreateVacunoCommand , CreateVacunoOutput> logging,
            AuditBehavior<CreateVacunoCommand , CreateVacunoOutput> audit,

            ICreateVacunoInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<CreateVacunoCommand , CreateVacunoOutput> Create()
        {
            return new BehaviorPipeline<CreateVacunoCommand , CreateVacunoOutput>(
            [
                _validation,
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}