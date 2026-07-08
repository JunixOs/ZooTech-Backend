using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.GetFecundacionEstado
{
    public class GetFecundacionEstadoBehaviorPipelineFactory : IGetFecundacionEstadoBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetFecundacionEstadoCommand , GetFecundacionEstadoOutput> _logging;
        private readonly AuditBehavior<GetFecundacionEstadoCommand , GetFecundacionEstadoOutput> _audit;

        private readonly IGetFecundacionEstadoInputPort _handler;

        public GetFecundacionEstadoBehaviorPipelineFactory(
            LoggingBehavior<GetFecundacionEstadoCommand , GetFecundacionEstadoOutput> logging,
            AuditBehavior<GetFecundacionEstadoCommand , GetFecundacionEstadoOutput> audit,

            IGetFecundacionEstadoInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetFecundacionEstadoCommand , GetFecundacionEstadoOutput> Create()
        {
            return new BehaviorPipeline<GetFecundacionEstadoCommand , GetFecundacionEstadoOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}