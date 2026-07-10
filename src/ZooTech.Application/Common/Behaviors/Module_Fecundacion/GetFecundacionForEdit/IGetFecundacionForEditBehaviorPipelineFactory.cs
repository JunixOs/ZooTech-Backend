using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.GetFecundacionForEdit
{
    public interface IGetFecundacionForEditBehaviorPipelineFactory
    {
        BehaviorPipeline<GetFecundacionForEditCommand , GetFecundacionForEditOutput> Create();
    }
}