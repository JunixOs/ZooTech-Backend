using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosExcel
{
    public class GenerateOrdeniosExcelBehaviorPipelineFactory : IGenerateOrdeniosExcelBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GenerateOrdeniosComparationExcelQuery , GenerateOrdeniosExcelOutput> _logging;
        private readonly AuditBehavior<GenerateOrdeniosComparationExcelQuery , GenerateOrdeniosExcelOutput> _audit;

        private readonly IGetOrdeniosExcelInputPort _handler;

        public GenerateOrdeniosExcelBehaviorPipelineFactory(
            LoggingBehavior<GenerateOrdeniosComparationExcelQuery , GenerateOrdeniosExcelOutput> logging,
            AuditBehavior<GenerateOrdeniosComparationExcelQuery , GenerateOrdeniosExcelOutput> audit,

            IGetOrdeniosExcelInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GenerateOrdeniosComparationExcelQuery , GenerateOrdeniosExcelOutput> Create()
        {
            return new BehaviorPipeline<GenerateOrdeniosComparationExcelQuery , GenerateOrdeniosExcelOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}