using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;

public interface IGetComparacionCelosRealVsEstandarInputPort 
    : IRequestHandler<GetComparacionCelosRealVsEstandarQuery , GetComparacionCelosRealVsEstandarOutput>
{
}