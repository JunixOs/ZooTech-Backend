using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandarPorVacuno
{
    public interface IGetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory
    {
        BehaviorPipeline<GetComparacionCelosRealVsEstandarPorVacunoCommand , GetComparacionCelosRealVsEstandarPorVacunoOutput> Create();
    }
}