using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandar
{
    public interface IGetComparacionCelosRealVsEstandarBehaviorPipelineFactory
    {
        BehaviorPipeline<GetComparacionCelosRealVsEstandarCommand , GetComparacionCelosRealVsEstandarOutput> Create();
    }
}