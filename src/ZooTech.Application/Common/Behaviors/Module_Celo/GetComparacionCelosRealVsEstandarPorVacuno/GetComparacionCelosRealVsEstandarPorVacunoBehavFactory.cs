using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandarPorVacuno
{
    public class GetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory : IGetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetComparacionCelosRealVsEstandarPorVacunoCommand , GetComparacionCelosRealVsEstandarPorVacunoOutput> _logging;
        private readonly AuditBehavior<GetComparacionCelosRealVsEstandarPorVacunoCommand , GetComparacionCelosRealVsEstandarPorVacunoOutput> _audit;

        private readonly IGetComparacionCelosRealVsEstandarPorVacunoInputPort _handler;

        public GetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory(
            LoggingBehavior<GetComparacionCelosRealVsEstandarPorVacunoCommand , GetComparacionCelosRealVsEstandarPorVacunoOutput> logging,
            AuditBehavior<GetComparacionCelosRealVsEstandarPorVacunoCommand , GetComparacionCelosRealVsEstandarPorVacunoOutput> audit,

            IGetComparacionCelosRealVsEstandarPorVacunoInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetComparacionCelosRealVsEstandarPorVacunoCommand , GetComparacionCelosRealVsEstandarPorVacunoOutput> Create()
        {
            return new BehaviorPipeline<GetComparacionCelosRealVsEstandarPorVacunoCommand , GetComparacionCelosRealVsEstandarPorVacunoOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}