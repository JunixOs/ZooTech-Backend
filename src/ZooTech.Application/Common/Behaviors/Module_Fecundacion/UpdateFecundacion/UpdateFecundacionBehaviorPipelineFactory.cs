using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.UpdateFecundacion
{
    public class UpdateFecundacionBehaviorPipelineFactory : IUpdateFecundacionBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<UpdateFecundacionCommand , UpdateFecundacionOutput> _validation;
        private readonly LoggingBehavior<UpdateFecundacionCommand , UpdateFecundacionOutput> _logging;
        private readonly AuditBehavior<UpdateFecundacionCommand , UpdateFecundacionOutput> _audit;

        private readonly IUpdateFecundacionInputPort _handler;

        public UpdateFecundacionBehaviorPipelineFactory(
            ValidationBehavior<UpdateFecundacionCommand , UpdateFecundacionOutput> validation,
            LoggingBehavior<UpdateFecundacionCommand , UpdateFecundacionOutput> logging,
            AuditBehavior<UpdateFecundacionCommand , UpdateFecundacionOutput> audit,

            IUpdateFecundacionInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<UpdateFecundacionCommand , UpdateFecundacionOutput> Create()
        {
            return new BehaviorPipeline<UpdateFecundacionCommand , UpdateFecundacionOutput>(
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