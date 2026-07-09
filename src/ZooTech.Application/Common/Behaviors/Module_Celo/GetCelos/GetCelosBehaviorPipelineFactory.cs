using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetCelos
{
    public class GetCelosBehaviorPipelineFactory : IGetCelosBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommand , GetCelosOutput> _logging;
        private readonly AuditBehavior<EmptyCommand , GetCelosOutput> _audit;

        private readonly IGetCelosInputPort _handler;

        public GetCelosBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommand , GetCelosOutput> logging,
            AuditBehavior<EmptyCommand , GetCelosOutput> audit,

            IGetCelosInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommand , GetCelosOutput> Create()
        {
            return new BehaviorPipeline<EmptyCommand , GetCelosOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}