using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialGeneral
{
    public class GetHistorialGeneralBehaviorPipelineFactory : IGetHistorialGeneralBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetHistorialGeneralCommand , GetHistorialGeneralOutput> _logging;
        private readonly AuditBehavior<GetHistorialGeneralCommand , GetHistorialGeneralOutput> _audit;

        private readonly IGetHistorialGeneralInputPort _handler;

        public GetHistorialGeneralBehaviorPipelineFactory(
            LoggingBehavior<GetHistorialGeneralCommand , GetHistorialGeneralOutput> logging,
            AuditBehavior<GetHistorialGeneralCommand , GetHistorialGeneralOutput> audit,

            IGetHistorialGeneralInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetHistorialGeneralCommand , GetHistorialGeneralOutput> Create()
        {
            return new BehaviorPipeline<GetHistorialGeneralCommand , GetHistorialGeneralOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}