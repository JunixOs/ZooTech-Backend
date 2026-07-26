using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllVacunosSanidad
{
    public interface IGetAllVacunosSanidadBehaviorPipelineFactory
    {
        BehaviorPipeline<EmptyCommandQuery , GetAllVacunosSanidadOutput> Create();
    }
}