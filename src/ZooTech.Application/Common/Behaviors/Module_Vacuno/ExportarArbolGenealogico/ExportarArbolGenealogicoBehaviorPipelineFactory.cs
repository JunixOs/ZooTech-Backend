using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ExportarArbolGenealogico
{
    public class ExportarArbolGenealogicoBehaviorPipelineFactory : IExportarArbolGenealogicoBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<ExportarArbolGenealogicoCommand , ExportarArbolGenealogicoOutput> _validation;
        private readonly LoggingBehavior<ExportarArbolGenealogicoCommand , ExportarArbolGenealogicoOutput> _logging;
        private readonly AuditBehavior<ExportarArbolGenealogicoCommand , ExportarArbolGenealogicoOutput> _audit;

        private readonly IExportarArbolGenealogicoInputPort _handler;

        public ExportarArbolGenealogicoBehaviorPipelineFactory(
            ValidationBehavior<ExportarArbolGenealogicoCommand , ExportarArbolGenealogicoOutput> validation,
            LoggingBehavior<ExportarArbolGenealogicoCommand , ExportarArbolGenealogicoOutput> logging,
            AuditBehavior<ExportarArbolGenealogicoCommand , ExportarArbolGenealogicoOutput> audit,

            IExportarArbolGenealogicoInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<ExportarArbolGenealogicoCommand , ExportarArbolGenealogicoOutput> Create()
        {
            return new BehaviorPipeline<ExportarArbolGenealogicoCommand , ExportarArbolGenealogicoOutput>(
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