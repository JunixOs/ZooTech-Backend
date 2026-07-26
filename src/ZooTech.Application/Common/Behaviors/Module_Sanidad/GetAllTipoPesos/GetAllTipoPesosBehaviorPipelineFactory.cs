using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTipoPesos
{
    public class GetAllTipoPesosBehaviorPipelineFactory : IGetAllTipoPesosBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommandQuery , GetAllTipoPesosOutput> _logging;
        private readonly AuditBehavior<EmptyCommandQuery , GetAllTipoPesosOutput> _audit;

        private readonly IGetAllTipoPesosInputPort _handler;

        public GetAllTipoPesosBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommandQuery , GetAllTipoPesosOutput> logging,
            AuditBehavior<EmptyCommandQuery , GetAllTipoPesosOutput> audit,

            IGetAllTipoPesosInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommandQuery , GetAllTipoPesosOutput> Create()
        {
            return new BehaviorPipeline<EmptyCommandQuery , GetAllTipoPesosOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}