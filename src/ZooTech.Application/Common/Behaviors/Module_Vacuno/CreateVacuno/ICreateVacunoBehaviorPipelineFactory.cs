using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.CreateVacuno
{
    public interface ICreateVacunoBehaviorPipelineFactory
    {
        BehaviorPipeline<CreateVacunoCommand , CreateVacunoOutput> Create();
    }
}