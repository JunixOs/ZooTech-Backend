using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.UpdateVacuno
{
    public interface IUpdateVacunoBehaviorPipelineFactory
    {
        BehaviorPipeline<UpdateVacunoCommand , UpdateVacunoOutput> Create();
    }
}