using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.DeleteVacuno
{
    public class DeleteVacunoBehaviorPipelineFactory : IDeleteVacunoBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<DeleteVacunoCommand , EmptyOutput> _validation;
        private readonly LoggingBehavior<DeleteVacunoCommand , EmptyOutput> _logging;
        private readonly AuditBehavior<DeleteVacunoCommand , EmptyOutput> _audit;

        private readonly IDeleteVacunoInputPort _handler;

        public DeleteVacunoBehaviorPipelineFactory(
            ValidationBehavior<DeleteVacunoCommand , EmptyOutput> validation,
            LoggingBehavior<DeleteVacunoCommand , EmptyOutput> logging,
            AuditBehavior<DeleteVacunoCommand , EmptyOutput> audit,

            IDeleteVacunoInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<DeleteVacunoCommand , EmptyOutput> Create()
        {
            return new BehaviorPipeline<DeleteVacunoCommand , EmptyOutput>(
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