using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTriajes
{
    public class GetAllTriajesBehaviorPipelineFactory : IGetAllTriajesBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetAllTriajesQuery , GetAllTriajesOutput> _logging;
        private readonly AuditBehavior<GetAllTriajesQuery , GetAllTriajesOutput> _audit;

        private readonly IGetAllTriajesInputPort _handler;

        public GetAllTriajesBehaviorPipelineFactory(
            LoggingBehavior<GetAllTriajesQuery , GetAllTriajesOutput> logging,
            AuditBehavior<GetAllTriajesQuery , GetAllTriajesOutput> audit,

            IGetAllTriajesInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetAllTriajesQuery , GetAllTriajesOutput> Create()
        {
            return new BehaviorPipeline<GetAllTriajesQuery , GetAllTriajesOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}