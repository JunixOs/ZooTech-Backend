using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.UpdateVacuno
{
    public class UpdateVacunoBehaviorPipelineFactory : IUpdateVacunoBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<UpdateVacunoCommand , UpdateVacunoOutput> _validation;
        private readonly LoggingBehavior<UpdateVacunoCommand , UpdateVacunoOutput> _logging;
        private readonly AuditBehavior<UpdateVacunoCommand , UpdateVacunoOutput> _audit;

        private readonly IUpdateVacunoInputPort _handler;

        public UpdateVacunoBehaviorPipelineFactory(
            ValidationBehavior<UpdateVacunoCommand , UpdateVacunoOutput> validation,
            LoggingBehavior<UpdateVacunoCommand , UpdateVacunoOutput> logging,
            AuditBehavior<UpdateVacunoCommand , UpdateVacunoOutput> audit,

            IUpdateVacunoInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<UpdateVacunoCommand , UpdateVacunoOutput> Create()
        {
            return new BehaviorPipeline<UpdateVacunoCommand , UpdateVacunoOutput>(
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