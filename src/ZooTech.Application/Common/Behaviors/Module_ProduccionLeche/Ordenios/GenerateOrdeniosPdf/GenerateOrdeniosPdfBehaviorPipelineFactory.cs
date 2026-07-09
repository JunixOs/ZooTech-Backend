using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosPdf
{
    public class GenerateOrdeniosPdfBehaviorPipelineFactory : IGenerateOrdeniosPdfBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GenerateOrdeniosComparationPdfQuery , GenerateOrdeniosPdfOutput> _logging;
        private readonly AuditBehavior<GenerateOrdeniosComparationPdfQuery , GenerateOrdeniosPdfOutput> _audit;

        private readonly IGetOrdeniosPdfInputPort _handler;

        public GenerateOrdeniosPdfBehaviorPipelineFactory(
            LoggingBehavior<GenerateOrdeniosComparationPdfQuery , GenerateOrdeniosPdfOutput> logging,
            AuditBehavior<GenerateOrdeniosComparationPdfQuery , GenerateOrdeniosPdfOutput> audit,

            IGetOrdeniosPdfInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GenerateOrdeniosComparationPdfQuery , GenerateOrdeniosPdfOutput> Create()
        {
            return new BehaviorPipeline<GenerateOrdeniosComparationPdfQuery , GenerateOrdeniosPdfOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}