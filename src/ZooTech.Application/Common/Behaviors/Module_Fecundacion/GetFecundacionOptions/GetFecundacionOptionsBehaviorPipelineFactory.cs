using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.GetFecundacionOptions
{
    public class GetFecundacionOptionsBehaviorPipelineFactory : IGetFecundacionOptionsBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommand , GetFecundacionOptionsOutput> _logging;
        private readonly AuditBehavior<EmptyCommand , GetFecundacionOptionsOutput> _audit;

        private readonly IGetFecundacionOptionsInputPort _handler;

        public GetFecundacionOptionsBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommand , GetFecundacionOptionsOutput> logging,
            AuditBehavior<EmptyCommand , GetFecundacionOptionsOutput> audit,

            IGetFecundacionOptionsInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommand , GetFecundacionOptionsOutput> Create()
        {
            return new BehaviorPipeline<EmptyCommand , GetFecundacionOptionsOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}