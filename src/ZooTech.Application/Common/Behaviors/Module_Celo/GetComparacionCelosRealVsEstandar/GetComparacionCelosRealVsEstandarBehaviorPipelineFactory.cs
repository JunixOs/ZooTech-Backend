using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandar
{
    public class GetComparacionCelosRealVsEstandarBehaviorPipelineFactory : IGetComparacionCelosRealVsEstandarBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<GetComparacionCelosRealVsEstandarCommand , GetComparacionCelosRealVsEstandarOutput> _logging;
        private readonly AuditBehavior<GetComparacionCelosRealVsEstandarCommand , GetComparacionCelosRealVsEstandarOutput> _audit;

        private readonly IGetComparacionCelosRealVsEstandarInputPort _handler;

        public GetComparacionCelosRealVsEstandarBehaviorPipelineFactory(
            LoggingBehavior<GetComparacionCelosRealVsEstandarCommand , GetComparacionCelosRealVsEstandarOutput> logging,
            AuditBehavior<GetComparacionCelosRealVsEstandarCommand , GetComparacionCelosRealVsEstandarOutput> audit,

            IGetComparacionCelosRealVsEstandarInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<GetComparacionCelosRealVsEstandarCommand , GetComparacionCelosRealVsEstandarOutput> Create()
        {
            return new BehaviorPipeline<GetComparacionCelosRealVsEstandarCommand , GetComparacionCelosRealVsEstandarOutput>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}