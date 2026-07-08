using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.UpdateFecundacionEstado
{
    public class UpdateFecundacionEstadoBehaviorPipelineFactory : IUpdateFecundacionEstadoBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput> _validation;
        private readonly LoggingBehavior<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput> _logging;
        private readonly AuditBehavior<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput> _audit;

        private readonly IUpdateFecundacionEstadoInputPort _handler;

        public UpdateFecundacionEstadoBehaviorPipelineFactory(
            ValidationBehavior<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput> validation,
            LoggingBehavior<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput> logging,
            AuditBehavior<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput> audit,

            IUpdateFecundacionEstadoInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput> Create()
        {
            return new BehaviorPipeline<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput>(
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