using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.GetVacunoById
{
    public interface IGetVacunoByIdBehaviorPipelineFactory
    {
        BehaviorPipeline<GetVacunoByIdCommand , GetVacunoByIdOutput> Create();
    }
}