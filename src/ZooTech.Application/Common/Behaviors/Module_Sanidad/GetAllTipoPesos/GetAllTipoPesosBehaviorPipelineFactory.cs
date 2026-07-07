using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTipoPesos
{
    public class GetAllTipoPesosBehaviorPipelineFactory : IGetAllTipoPesosBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommand , GetAllTipoPesosOutput> _logging;
        private readonly AuditBehavior<EmptyCommand , GetAllTipoPesosOutput> _audit;

        private readonly IGetAllTipoPesosInputPort _handler;

        public GetAllTipoPesosBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommand , GetAllTipoPesosOutput> logging,
            AuditBehavior<EmptyCommand , GetAllTipoPesosOutput> audit,

            IGetAllTipoPesosInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommand , GetAllTipoPesosOutput> Create()
        {
            return new BehaviorPipeline<EmptyCommand , GetAllTipoPesosOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}