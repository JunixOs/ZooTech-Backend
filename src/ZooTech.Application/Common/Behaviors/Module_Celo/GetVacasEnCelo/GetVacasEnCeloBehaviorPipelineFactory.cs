using ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetVacasEnCelo
{
    public class GetVacasEnCeloBehaviorPipelineFactory : IGetVacasEnCeloBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetVacasEnCeloCommand , GetVacasEnCeloOutput> _logging;
        private readonly AuditBehavior<GetVacasEnCeloCommand , GetVacasEnCeloOutput> _audit;

        private readonly IGetVacasEnCeloInputPort _handler;

        public GetVacasEnCeloBehaviorPipelineFactory(
            LoggingBehavior<GetVacasEnCeloCommand , GetVacasEnCeloOutput> logging,
            AuditBehavior<GetVacasEnCeloCommand , GetVacasEnCeloOutput> audit,

            IGetVacasEnCeloInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetVacasEnCeloCommand , GetVacasEnCeloOutput> Create()
        {
            return new BehaviorPipeline<GetVacasEnCeloCommand , GetVacasEnCeloOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}