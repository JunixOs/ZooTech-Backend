using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.GetFecundacionForEdit
{
    public class GetFecundacionForEditBehaviorPipelineFactory : IGetFecundacionForEditBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetFecundacionForEditCommand , GetFecundacionForEditOutput> _logging;
        private readonly AuditBehavior<GetFecundacionForEditCommand , GetFecundacionForEditOutput> _audit;

        private readonly IGetFecundacionForEditInputPort _handler;

        public GetFecundacionForEditBehaviorPipelineFactory(
            LoggingBehavior<GetFecundacionForEditCommand , GetFecundacionForEditOutput> logging,
            AuditBehavior<GetFecundacionForEditCommand , GetFecundacionForEditOutput> audit,

            IGetFecundacionForEditInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetFecundacionForEditCommand , GetFecundacionForEditOutput> Create()
        {
            return new BehaviorPipeline<GetFecundacionForEditCommand , GetFecundacionForEditOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}