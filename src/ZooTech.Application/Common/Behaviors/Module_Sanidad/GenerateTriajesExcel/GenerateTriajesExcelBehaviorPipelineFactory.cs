using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesExcel
{
    public class GenerateTriajesExcelBehaviorPipelineFactory : IGenerateTriajesExcelBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GenerateTriajesExcelQuery , GenerateTriajesExcelOutput> _logging;
        private readonly AuditBehavior<GenerateTriajesExcelQuery , GenerateTriajesExcelOutput> _audit;

        private readonly IGenerateTriajesExcelInputPort _handler;

        public GenerateTriajesExcelBehaviorPipelineFactory(
            LoggingBehavior<GenerateTriajesExcelQuery , GenerateTriajesExcelOutput> logging,
            AuditBehavior<GenerateTriajesExcelQuery , GenerateTriajesExcelOutput> audit,

            IGenerateTriajesExcelInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GenerateTriajesExcelQuery , GenerateTriajesExcelOutput> Create()
        {
            return new BehaviorPipeline<GenerateTriajesExcelQuery , GenerateTriajesExcelOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}