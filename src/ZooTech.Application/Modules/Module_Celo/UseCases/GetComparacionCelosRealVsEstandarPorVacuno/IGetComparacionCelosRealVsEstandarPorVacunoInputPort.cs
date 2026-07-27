using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;

public interface IGetComparacionCelosRealVsEstandarPorVacunoInputPort
    : IRequestHandler<GetComparacionCelosRealVsEstandarPorVacunoQuery , GetComparacionCelosRealVsEstandarPorVacunoOutput>
{
}
