using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesPdf
{
    public class GenerateTriajesPdfBehaviorPipelineFactory : IGenerateTriajesPdfBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GenerateTriajesPdfQuery , GenerateTriajesPdfOutput> _logging;
        private readonly AuditBehavior<GenerateTriajesPdfQuery , GenerateTriajesPdfOutput> _audit;

        private readonly IGenerateTriajesPdfInputPort _handler;

        public GenerateTriajesPdfBehaviorPipelineFactory(
            LoggingBehavior<GenerateTriajesPdfQuery , GenerateTriajesPdfOutput> logging,
            AuditBehavior<GenerateTriajesPdfQuery , GenerateTriajesPdfOutput> audit,

            IGenerateTriajesPdfInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GenerateTriajesPdfQuery , GenerateTriajesPdfOutput> Create()
        {
            return new BehaviorPipeline<GenerateTriajesPdfQuery , GenerateTriajesPdfOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}