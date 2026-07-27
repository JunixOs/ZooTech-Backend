using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetHistorialCeloPorVacuno;

public interface IGetHistorialCeloPorVacunoBehaviorPipelineFactory
{
    BehaviorPipeline<GetHistorialCeloPorVacunoCommand, GetHistorialCeloPorVacunoOutput> Create();
}
