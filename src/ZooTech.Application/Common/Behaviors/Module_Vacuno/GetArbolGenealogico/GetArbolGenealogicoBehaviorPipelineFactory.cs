using ZooTech.Application.Common.Behaviors.Module_Vacuno.GetArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ExportarArbolGenealogico
{
    public class GetArbolGenealogicoBehaviorPipelineFactory : IGetArbolGenealogicoBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<GetArbolGenealogicoCommand , GetArbolGenealogicoOutput> _validation;
        private readonly LoggingBehavior<GetArbolGenealogicoCommand , GetArbolGenealogicoOutput> _logging;
        private readonly AuditBehavior<GetArbolGenealogicoCommand , GetArbolGenealogicoOutput> _audit;

        private readonly IGetArbolGenealogicoInputPort _handler;

        public GetArbolGenealogicoBehaviorPipelineFactory(
            ValidationBehavior<GetArbolGenealogicoCommand , GetArbolGenealogicoOutput> validation,
            LoggingBehavior<GetArbolGenealogicoCommand , GetArbolGenealogicoOutput> logging,
            AuditBehavior<GetArbolGenealogicoCommand , GetArbolGenealogicoOutput> audit,

            IGetArbolGenealogicoInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetArbolGenealogicoCommand , GetArbolGenealogicoOutput> Create()
        {
            return new BehaviorPipeline<GetArbolGenealogicoCommand , GetArbolGenealogicoOutput>(
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