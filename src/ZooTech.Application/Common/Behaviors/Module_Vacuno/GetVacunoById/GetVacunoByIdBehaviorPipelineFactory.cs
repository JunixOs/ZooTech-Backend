using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.GetVacunoById
{
    public class GetVacunoByIdBehaviorPipelineFactory : IGetVacunoByIdBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetVacunoByIdCommand , GetVacunoByIdOutput> _logging;
        private readonly AuditBehavior<GetVacunoByIdCommand , GetVacunoByIdOutput> _audit;

        private readonly IGetVacunoByIdInputPort _handler;

        public GetVacunoByIdBehaviorPipelineFactory(
            LoggingBehavior<GetVacunoByIdCommand , GetVacunoByIdOutput> logging,
            AuditBehavior<GetVacunoByIdCommand , GetVacunoByIdOutput> audit,

            IGetVacunoByIdInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetVacunoByIdCommand , GetVacunoByIdOutput> Create()
        {
            return new BehaviorPipeline<GetVacunoByIdCommand , GetVacunoByIdOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}